using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.Repository;

namespace Municiple_Project_st10259527.Controllers
{
    public class EventsAndAnnouncements : Controller
    {

        private readonly IEventsRepository _eventsRepository;
        private readonly IAnnouncementsRepository _announcementsRepository;

        public EventsAndAnnouncements(IEventsRepository eventsRepository, IAnnouncementsRepository announcementsRepository)
        {
            _eventsRepository = eventsRepository;
            _announcementsRepository = announcementsRepository;
        }

        public IActionResult EventsDashboard()
        {
            return View();
        }

        

        [HttpPost]
        public IActionResult AddAnnouncement(Models.AnnouncementModel announcement)
        {
            if (ModelState.IsValid)
            {
                _announcementsRepository.AddAnnouncement(announcement);
                return RedirectToAction("Announcements/ManageAnnouncements");
            }
            return View("Announcements/ManageAnnouncements", announcement);
        }
    }
}
