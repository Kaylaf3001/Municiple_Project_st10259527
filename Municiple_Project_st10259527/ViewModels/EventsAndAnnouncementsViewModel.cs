using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.ViewModels
{
    public class EventsAndAnnouncementsViewModel
    {
        public IEnumerable<EventModel> Events { get; set; } = new List<EventModel>();
        public IEnumerable<AnnouncementModel> Announcements { get; set; } = new List<AnnouncementModel>();
        public Queue<EventModel> UpcomingEventsQueue { get; set; } = new Queue<EventModel>();
    }
}
