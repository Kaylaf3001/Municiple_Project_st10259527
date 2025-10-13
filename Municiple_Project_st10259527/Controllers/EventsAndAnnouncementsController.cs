using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.ViewModels;
using System.Diagnostics;
using System.Text.Json;

namespace Municiple_Project_st10259527.Controllers
{

    public class EventsAndAnnouncementsController : Controller
    {
        //===============================================================================================
        // Dependency Injection for Repositories
        //===============================================================================================
        #region
        private readonly IEventsRepository _eventsRepository;
        private readonly IAnnouncementsRepository _announcementsRepository;
        private readonly EventAnnouncementService _eventAnnouncementService;
        private readonly RecommendationService _recommendationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EventsAndAnnouncementsController> _logger;
        private const string RecentlyViewedSessionKey = "RecentlyViewedStack";

        public EventsAndAnnouncementsController(
            IEventsRepository eventsRepository, 
            IAnnouncementsRepository announcementsRepository, 
            EventAnnouncementService eventAnnouncementService,
            RecommendationService recommendationService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<EventsAndAnnouncementsController> logger)
        {
            _eventsRepository = eventsRepository;
            _announcementsRepository = announcementsRepository;
            _eventAnnouncementService = eventAnnouncementService;
            _recommendationService = recommendationService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
        }

        private Stack<int> GetRecentlyViewedStack()
        {
            var json = _httpContextAccessor.HttpContext?.Session.GetString(RecentlyViewedSessionKey);
            if (string.IsNullOrEmpty(json)) return new Stack<int>();
            try
            {
                var ids = JsonSerializer.Deserialize<Stack<int>>(json);
                return ids ?? new Stack<int>();
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

        private void PushRecentlyViewed(int eventId, int maxSize = 5)
        {
            var stack = GetRecentlyViewedStack();
            // Avoid consecutive duplicates
            if (stack.Count == 0 || stack.Peek() != eventId)
            {
                stack.Push(eventId);
                while (stack.Count > maxSize)
                {
                    // Remove oldest by reversing into a temp stack
                    var temp = new Stack<int>();
                    while (stack.Count > 1) temp.Push(stack.Pop());
                    // Discard the bottom element
                    stack.Pop();
                    while (temp.Count > 0) stack.Push(temp.Pop());
                }
                SaveRecentlyViewedStack(stack);
            }
        }
        #endregion
        //===============================================================================================

        // ===============================================================================================
        // GET: EventsAndAnnouncements/EventsDashboard
        // Handles both regular view and filtering
        //===============================================================================================
        public async Task<IActionResult> EventsDashboard(
    string selectedCategory = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    string searchTerm = null)
        {
            try
            {
                // Get all events and announcements
                var allEvents = await _eventsRepository.GetAllEventsAsync();
                var allAnnouncements = await _announcementsRepository.GetAllAnnouncementsAsync();

                // Convert to HashSet for efficient lookups
                var eventsSet = new HashSet<EventModel>(allEvents, new EventModelComparer());

                // Filter events based on search criteria
                var filteredEvents = eventsSet.AsQueryable();

                // Log the search if there's a search term and a logged-in user
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var currentUserId = GetCurrentUserId();
                    if (currentUserId.HasValue)
                    {
                        try
                        {
                            string category = SearchCategories.GetCategoryForTerm(searchTerm);
                            _logger.LogInformation("Logging search - UserId: {UserId}, SearchTerm: {SearchTerm}, Category: {Category}",
                                currentUserId, searchTerm, category);

                            await _recommendationService.LogSearchAsync(currentUserId.Value, searchTerm, category);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error logging search for UserId: {UserId}, SearchTerm: {SearchTerm}",
                                currentUserId, searchTerm);
                            // Continue with the search even if logging fails
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Search performed by unauthenticated user - SearchTerm: {SearchTerm}", searchTerm);
                    }

                    // Apply search filter
                    var search = searchTerm.Trim().ToLower();
                    filteredEvents = filteredEvents
                        .Where(e => (e.Title != null && e.Title.ToLower().Contains(search)) ||
                                  (e.Description != null && e.Description.ToLower().Contains(search)) ||
                                  (e.Location != null && e.Location.ToLower().Contains(search)) ||
                                  (e.Category != null && e.Category.ToLower().Contains(search)));
                }

                // Apply category filter
                if (!string.IsNullOrEmpty(selectedCategory))
                {
                    filteredEvents = filteredEvents
                        .Where(e => e.Category != null &&
                                  e.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase));
                }

                // Apply date filters
                if (startDate.HasValue)
                {
                    filteredEvents = filteredEvents.Where(e => e.Date >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    filteredEvents = filteredEvents.Where(e => e.Date <= endDate.Value);
                }

                // Get recommended events - preserve the ordering provided by the service
                IEnumerable<EventModel> recommendationsOrdered = Enumerable.Empty<EventModel>();
                var currentUserIdForRecommendations = GetCurrentUserId();
                if (currentUserIdForRecommendations.HasValue)
                {
                    recommendationsOrdered = await _recommendationService.GetRecommendedEventsAsync(currentUserIdForRecommendations.Value);
                }

                // Get unique categories
                var uniqueCategories = new HashSet<string>(
                    allEvents
                        .Where(e => !string.IsNullOrEmpty(e.Category))
                        .Select(e => e.Category)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(c => c),
                    StringComparer.OrdinalIgnoreCase
                );

                // Group events by date using queues
                var eventsByDate = new Dictionary<DateTime, Queue<EventModel>>();
                foreach (var evt in filteredEvents.Where(e => e.Date >= DateTime.Today))
                {
                    var date = evt.Date.Date;
                    if (!eventsByDate.ContainsKey(date))
                    {
                        eventsByDate[date] = new Queue<EventModel>();
                    }
                    eventsByDate[date].Enqueue(evt);
                }

                // Build recently viewed events from session-backed stack (LIFO order)
                var recentStack = GetRecentlyViewedStack();
                var recentEvents = new List<EventModel>();
                foreach (var id in recentStack)
                {
                    var e = await _eventsRepository.GetEventByIdAsync(id);
                    if (e != null) recentEvents.Add(e);
                }

                // Create view model
                var viewModel = new EventsAndAnnouncementsViewModel
                {
                    Events = filteredEvents
                        .OrderBy(e => e.Date)
                        .ThenBy(e => e.Title)
                        .AsEnumerable(),

                    Announcements = allAnnouncements
                        .OrderByDescending(a => a.Date)
                        .AsEnumerable(),

                    RecommendedEvents = recommendationsOrdered,

                    SelectedCategory = selectedCategory,
                    StartDate = startDate,
                    EndDate = endDate,
                    SearchTerm = searchTerm,

                    // Set unique categories (case-insensitive)
                    UniqueCategories = uniqueCategories,

                    // Group events by category
                    EventsByCategory = allEvents
                        .Where(e => !string.IsNullOrEmpty(e.Category))
                        .GroupBy(e => e.Category, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(
                            g => g.Key,
                            g => new HashSet<EventModel>(g, new EventModelComparer()),
                            StringComparer.OrdinalIgnoreCase
                        ),

                    // Set events by date
                    EventsByDate = eventsByDate,

                    // Set queue count (if applicable)
                    QueueCount = allEvents.Count(e => e.Status == "Pending"),
                    RecentlyViewedEvents = recentEvents
                };

                // Set next upcoming event
                viewModel.NextUpcomingEvent = allEvents
                    .Where(e => e.Date >= DateTime.Now)
                    .OrderBy(e => e.Date)
                    .FirstOrDefault();

                return View("EventsDashboard", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EventsDashboard");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // Simple endpoint to record a recently viewed event using a Stack<int> in session
        [HttpGet]
        public IActionResult ViewEvent(int id)
        {
            PushRecentlyViewed(id);
            return RedirectToAction(nameof(EventsDashboard));
        }

        // Lightweight endpoint for AJAX logging without redirect
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogView(int id)
        {
            try
            {
                // Get the event to ensure it exists
                var evt = await _eventsRepository.GetEventByIdAsync(id);
                if (evt == null)
                {
                    return NotFound();
                }

                // Update the recently viewed stack
                PushRecentlyViewed(id);

                // Get the updated stack and return the recent events
                var stack = GetRecentlyViewedStack();
                var recentEvents = new List<EventModel>();
                
                // Get the actual event details for each ID in the stack
                foreach (var eventId in stack.Take(5)) // Limit to 5 most recent
                {
                    var recentEvent = await _eventsRepository.GetEventByIdAsync(eventId);
                    if (recentEvent != null)
                    {
                        recentEvents.Add(recentEvent);
                    }
                }

                return Json(new { success = true, recentEvents });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging view for event {EventId}", id);
                return StatusCode(500, new { success = false, error = "An error occurred while updating recently viewed events." });
            }
        }

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
        
        //===============================================================================================

        //===============================================================================================
        // GET: Events/Edit
        //===============================================================================================
        public async Task<IActionResult> EditEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _eventsRepository.GetEventByIdAsync(id.Value);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View("EditEvent", eventModel);
        }
        //===============================================================================================

        // POST: Events/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, [Bind("EventId,Title,Location,Date,Category,Description,Status")] EventModel eventModel)
        {
            if (id != eventModel.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _eventsRepository.UpdateEventAsync(eventModel);
                if (result)
                {
                    TempData["SuccessMessage"] = "Event updated successfully!";
                    return RedirectToAction(nameof(EventsDashboard));
                }
                else
                {
                    ModelState.AddModelError("", "Unable to update event. Please try again.");
                }
            }
            return View("EditEvent", eventModel);
        }
        //===============================================================================================

        //===============================================================================================
        //DELETE: Events/Delete/id
        //===============================================================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEventConfirmed(int id)
        {
            var result = await _eventsRepository.DeleteEventAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to delete event. Please try again.";
            }
            return RedirectToAction("ManageEvents", "Events");
        }
        //===============================================================================================

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
        return obj?.EventId.GetHashCode() ?? 0;
    }
}
//================================End==Of==File===========================================================
