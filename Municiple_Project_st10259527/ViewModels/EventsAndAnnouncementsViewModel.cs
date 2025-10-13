using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.ViewModels
{
    public class EventsAndAnnouncementsViewModel
    {
        // Main collections
        public IEnumerable<EventModel> Events { get; set; } = new List<EventModel>();
        public IEnumerable<AnnouncementModel> Announcements { get; set; } = new List<AnnouncementModel>();
        
        // Next upcoming event (from the queue)
        public EventModel NextUpcomingEvent { get; set; }
        
        // Queue information
        public int QueueCount { get; set; }
        
        // Filtering properties
        public List<string> Categories { get; set; } = new();
        public string SelectedCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SearchTerm { get; set; }
        
        // Queue status properties
        public bool HasEventsInQueue => QueueCount > 0;
    }
}
