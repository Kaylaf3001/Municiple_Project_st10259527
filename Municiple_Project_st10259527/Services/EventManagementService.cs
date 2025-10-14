using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.ViewModels;

namespace Municiple_Project_st10259527.Services
{
    public class EventManagementService
    {
        //===============================================================================================
        // Dependency Injection for Repositories and Constants
        //===============================================================================================
        #region
        private readonly IEventsRepository _eventsRepository;
        private readonly RecommendationService _recommendationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string RecentlyViewedSessionKey = "RecentlyViewedStack";

        public EventManagementService(IEventsRepository eventsRepository,RecommendationService recommendationService,IHttpContextAccessor httpContextAccessor)
        {
            _eventsRepository = eventsRepository;
            _recommendationService = recommendationService;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion
        //===============================================================================================

        //==============================================================================================
        // Manage the Events Dashboard ViewModel
        //==============================================================================================
        public async Task<EventsAndAnnouncementsViewModel> GetEventsDashboardViewModelAsync(string selectedCategory = null,DateTime? startDate = null,DateTime? endDate = null,string searchTerm = null)
        {
            // Get all events from repository
            var allEvents = await _eventsRepository.GetAllEventsAsync();

            var filteredEvents = FilterEvents(allEvents, selectedCategory, startDate, endDate, searchTerm);

            // Group future events by date using Queue
            var eventsByDate = new Dictionary<DateTime, Queue<EventModel>>();
            foreach (var e in filteredEvents)
            {
                if (e.Date >= DateTime.Today)
                {
                    if (!eventsByDate.ContainsKey(e.Date.Date))
                        eventsByDate[e.Date.Date] = new Queue<EventModel>();
                    eventsByDate[e.Date.Date].Enqueue(e);
                }
            }

            // Group future events by category using HashSet
            var eventsByCategory = new Dictionary<string, HashSet<EventModel>>(StringComparer.OrdinalIgnoreCase);
            foreach (var e in filteredEvents)
            {
                if (e.Date >= DateTime.Today && !string.IsNullOrWhiteSpace(e.Category))
                {
                    if (!eventsByCategory.ContainsKey(e.Category))
                        eventsByCategory[e.Category] = new HashSet<EventModel>(new EventModelComparer());
                    eventsByCategory[e.Category].Add(e);
                }
            }

            // Get recently viewed events from session stack
            var recentEvents = await GetRecentlyViewedEventsAsync();

            // Get recommended events
            var recommendations = await GetRecommendedEventsAsync();

            return new EventsAndAnnouncementsViewModel
            {
                Events = filteredEvents,
                EventsByDate = eventsByDate,
                EventsByCategory = eventsByCategory,
                RecentlyViewedEvents = recentEvents,
                RecommendedEvents = recommendations,
                SelectedCategory = selectedCategory,
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = searchTerm,
                UniqueCategories = new HashSet<string>(allEvents
                    .Where(e => !string.IsNullOrEmpty(e.Category))
                    .Select(e => e.Category),
                    StringComparer.OrdinalIgnoreCase),
                NextUpcomingEvent = GetNextUpcomingEvent(filteredEvents)
            };
        }

        //==============================================================================================
        // Filtering and Helper Methods
        //==============================================================================================
        private IEnumerable<EventModel> FilterEvents(IEnumerable<EventModel> events,string category, DateTime? start,DateTime? end,string searchTerm)
        {
            foreach (var e in events)
            {
                if (!string.IsNullOrEmpty(category) &&
                    (e.Category == null || !e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)))
                    continue;

                if (start.HasValue && e.Date < start.Value)
                    continue;

                if (end.HasValue && e.Date > end.Value)
                    continue;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim();
                    if (!((e.Title?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                          (e.Description?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                          (e.Location?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                          (e.Category?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)))
                        continue;
                }

                yield return e;
            }
        }
        //==============================================================================================

        //==============================================================================================
        // Tracking Recently Viewed Events using Stacks
        //==============================================================================================
        public async Task LogEventViewAsync(int eventId)
        {
            var stack = GetRecentlyViewedStack();

            // Avoid consecutive duplicates
            if (stack.Count == 0 || stack.Peek() != eventId)
            {
                stack.Push(eventId);

                // Limit stack to 5 elements
                while (stack.Count > 5)
                {
                    var temp = new Stack<int>();
                    while (stack.Count > 1) temp.Push(stack.Pop());
                    stack.Pop(); // Remove bottom element
                    while (temp.Count > 0) stack.Push(temp.Pop());
                }

                SaveRecentlyViewedStack(stack);
            }
        }
        //==============================================================================================

        //==============================================================================================
        // Retrieve Recently Viewed Events, an internal helper to read from session
        //==============================================================================================
        private Stack<int> GetRecentlyViewedStack()
        {
            var json = _httpContextAccessor.HttpContext?.Session.GetString(RecentlyViewedSessionKey);
            if (string.IsNullOrEmpty(json)) return new Stack<int>();

            try
            {
                return JsonSerializer.Deserialize<Stack<int>>(json) ?? new Stack<int>();
            }
            catch
            {
                return new Stack<int>();
            }
        }
        //==============================================================================================

        //==============================================================================================
        // Gets the viewed events, it takes the id from the stack and fetches the event details from repository
        //==============================================================================================
        public async Task<IEnumerable<EventModel>> GetRecentlyViewedEventsAsync()
        {
            var stack = GetRecentlyViewedStack();
            var recentEvents = new List<EventModel>();
            
            foreach (var id in stack)
            {
                var e = await _eventsRepository.GetEventByIdAsync(id);
                if (e != null) recentEvents.Add(e);
            }
            
            return recentEvents;
        }
        //==============================================================================================

        //==============================================================================================
        // Saves the stack back to session
        //==============================================================================================
        private void SaveRecentlyViewedStack(Stack<int> stack)
        {
            var json = JsonSerializer.Serialize(stack);
            _httpContextAccessor.HttpContext?.Session.SetString(RecentlyViewedSessionKey, json);
        }
        //==============================================================================================

        //==============================================================================================
        // Get recommended events based on user's search history that is stored in the database
        //==============================================================================================
        private async Task<IEnumerable<EventModel>> GetRecommendedEventsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId.HasValue)
                return await _recommendationService.GetRecommendedEventsAsync(userId.Value);
            return new Stack<EventModel>();
        }
        //==============================================================================================

        //==============================================================================================
        // Get the next upcoming events
        //==============================================================================================
        private EventModel GetNextUpcomingEvent(IEnumerable<EventModel> events)
        {
            EventModel next = null;
            foreach (var e in events)
            {
                if (e.Date >= DateTime.Now)
                {
                    if (next == null || e.Date < next.Date) next = e;
                }
            }
            return next;
        }
        //==============================================================================================
    }

    //==============================================================================================
    // Custom comparer for EventModel to avoid duplicates in HashSet
    //==============================================================================================
    public class EventModelComparer : IEqualityComparer<EventModel>
    {
        public bool Equals(EventModel x, EventModel y) => x?.EventId == y?.EventId;
        public int GetHashCode(EventModel obj) => obj.EventId.GetHashCode();
    }
    //==============================================================================================
}
//==============================================End==Of==File============================================
