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

        public async Task<IActionResult> EventsDashboard()
        {
            try
            {
                var events = await _eventsRepository.GetAllEventsAsync();
                return View(events);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while loading events.");
            }
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
