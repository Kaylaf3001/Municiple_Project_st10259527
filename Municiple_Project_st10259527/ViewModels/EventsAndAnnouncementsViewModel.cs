using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;
using System.Linq;

namespace Municiple_Project_st10259527.ViewModels
{
    public class EventsAndAnnouncementsViewModel
    {
        // Main collections with optimized data structures
        public IEnumerable<EventModel> Events { get; set; } = Enumerable.Empty<EventModel>();
        public IEnumerable<AnnouncementModel> Announcements { get; set; } = Enumerable.Empty<AnnouncementModel>();

        // Grouped collections for efficient access
        public Dictionary<DateTime, Queue<EventModel>> EventsByDate { get; set; } = new(EqualityComparer<DateTime>.Default);
        public Dictionary<string, HashSet<EventModel>> EventsByCategory { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        // Unique categories for filtering
        public HashSet<string> UniqueCategories { get; set; } = new();

        // Next upcoming event (from the queue)
        public EventModel NextUpcomingEvent { get; set; }

        // Queue information
        public int QueueCount { get; set; }

        // Filtering properties
        public string SearchTerm { get; set; }
        public string SelectedCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Recommended events based on user's search history
        public IEnumerable<EventModel> RecommendedEvents { get; set; } = Enumerable.Empty<EventModel>();

        // Recently viewed events (stack-driven, displayed in sidebar)
        public IEnumerable<EventModel> RecentlyViewedEvents { get; set; } = Enumerable.Empty<EventModel>();

        // Helper properties
        public bool HasEventsInQueue => QueueCount > 0;
        public bool HasRecommendedEvents => RecommendedEvents.Any();
    }
}