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
        private readonly IEventsRepository _eventsRepository;
        private readonly IAnnouncementsRepository _announcementsRepository;
        private readonly RecommendationService _recommendationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string RecentlyViewedSessionKey = "RecentlyViewedStack";

        public EventManagementService(
            IEventsRepository eventsRepository,
            IAnnouncementsRepository announcementsRepository,
            RecommendationService recommendationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _eventsRepository = eventsRepository;
            _announcementsRepository = announcementsRepository;
            _recommendationService = recommendationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<EventsAndAnnouncementsViewModel> GetEventsDashboardViewModelAsync(
        string selectedCategory = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string searchTerm = null)
        {
            // Get all events and filter them
            var allEvents = (await _eventsRepository.GetAllEventsAsync()).ToList();
            var filteredEvents = FilterEvents(allEvents, selectedCategory, startDate, endDate, searchTerm).ToList();

            // Attempt to log this search/filter to build recommendation history
            await TryLogSearchAsync(selectedCategory, searchTerm);

            // Group events by date
            var eventsByDate = filteredEvents
                .Where(e => e.Date >= DateTime.Today)  // Only show future events
                .GroupBy(e => e.Date.Date)
                .ToDictionary(
                    g => g.Key,
                    g => new Queue<EventModel>(g.OrderBy(e => e.Date))
                );

            // Group events by category for sidebar counts (future events only)
            var eventsByCategory = filteredEvents
                .Where(e => e.Date >= DateTime.Today && !string.IsNullOrWhiteSpace(e.Category))
                .GroupBy(e => e.Category, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => new HashSet<EventModel>(g, new EventModelComparer()),
                    StringComparer.OrdinalIgnoreCase
                );

            // Get other required data
            var allAnnouncements = await _announcementsRepository.GetAllAnnouncementsAsync();
            var recommendations = await GetRecommendedEventsAsync();
            var recentEvents = await GetRecentlyViewedEventsAsync();

            // Create and return view model
            return new EventsAndAnnouncementsViewModel
            {
                Events = filteredEvents,
                EventsByDate = eventsByDate,
                EventsByCategory = eventsByCategory,
                Announcements = allAnnouncements.OrderByDescending(a => a.Date),
                RecommendedEvents = recommendations,
                SelectedCategory = selectedCategory,
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = searchTerm,
                UniqueCategories = allEvents
                    .Where(e => !string.IsNullOrEmpty(e.Category))
                    .Select(e => e.Category)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(c => c)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase),
                NextUpcomingEvent = allEvents
                    .Where(e => e.Date >= DateTime.Now)
                    .OrderBy(e => e.Date)
                    .FirstOrDefault(),
                RecentlyViewedEvents = recentEvents
            };
        }

        public async Task LogEventViewAsync(int eventId)
        {
            var stack = GetRecentlyViewedStack();
            
            // Avoid consecutive duplicates
            if (stack.Count == 0 || stack.Peek() != eventId)
            {
                stack.Push(eventId);
                
                // Limit to 5 most recent
                while (stack.Count > 5)
                {
                    var temp = new Stack<int>();
                    while (stack.Count > 1) temp.Push(stack.Pop());
                    stack.Pop(); // Discard the bottom element
                    while (temp.Count > 0) stack.Push(temp.Pop());
                }
                
                SaveRecentlyViewedStack(stack);
            }
        }

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

        private IEnumerable<EventModel> FilterEvents(
            IEnumerable<EventModel> events,
            string selectedCategory,
            DateTime? startDate,
            DateTime? endDate,
            string searchTerm)
        {
            var query = events.AsQueryable();

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                query = query.Where(e => 
                    e.Category != null && 
                    e.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            if (startDate.HasValue)
            {
                query = query.Where(e => e.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.Date <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(e => 
                    (e.Title != null && e.Title.ToLower().Contains(search)) ||
                    (e.Description != null && e.Description.ToLower().Contains(search)) ||
                    (e.Location != null && e.Location.ToLower().Contains(search)) ||
                    (e.Category != null && e.Category.ToLower().Contains(search)));
            }

            return query.OrderBy(e => e.Date).ThenBy(e => e.Title);
        }

        private async Task<IEnumerable<EventModel>> GetRecommendedEventsAsync()
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId.HasValue)
            {
                return await _recommendationService.GetRecommendedEventsAsync(currentUserId.Value);
            }
            return Enumerable.Empty<EventModel>();
        }

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

        private void SaveRecentlyViewedStack(Stack<int> stack)
        {
            var json = JsonSerializer.Serialize(stack);
            _httpContextAccessor.HttpContext?.Session.SetString(RecentlyViewedSessionKey, json);
        }

        private int? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
        }

        private async Task TryLogSearchAsync(string selectedCategory, string searchTerm)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return;
            }

            // Prefer explicit search term; if not present, log category as the term
            var termToLog = !string.IsNullOrWhiteSpace(searchTerm)
                ? searchTerm
                : (!string.IsNullOrWhiteSpace(selectedCategory) ? selectedCategory : null);

            if (string.IsNullOrWhiteSpace(termToLog))
            {
                return;
            }

            try
            {
                await _recommendationService.LogSearchAsync(userId.Value, termToLog, selectedCategory);
            }
            catch
            {
                // no-op: avoid breaking dashboard on logging errors
            }
        }
    }

    public class EventModelComparer : IEqualityComparer<EventModel>
    {
        public bool Equals(EventModel x, EventModel y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.EventId == y.EventId;
        }

        public int GetHashCode(EventModel obj)
        {
            return obj.EventId.GetHashCode();
        }
    }
}
