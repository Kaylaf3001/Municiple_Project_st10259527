using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.ViewModels;

namespace Municiple_Project_st10259527.Controllers
{

    public class EventsAndAnnouncements : Controller
    {
        //===============================================================================================
        // Dependency Injection for Repositories
        //===============================================================================================
        #region
        private readonly IEventsRepository _eventsRepository;
        private readonly IAnnouncementsRepository _announcementsRepository;

        public EventsAndAnnouncements(IEventsRepository eventsRepository, IAnnouncementsRepository announcementsRepository)
        {
            _eventsRepository = eventsRepository;
            _announcementsRepository = announcementsRepository;
        }
        #endregion
        //===============================================================================================

        //===============================================================================================
        // GET: EventsAndAnnouncements/EventsDashboard
        //===============================================================================================
        public async Task<IActionResult> EventsDashboard()
        {
            try
            {
                // Get both events and announcements
                var events = await _eventsRepository.GetAllEventsAsync();
                var announcements = await _announcementsRepository.GetAllAnnouncementsAsync();

                // Create and populate the ViewModel
                var viewModel = new EventsAndAnnouncementsViewModel
                {
                    Events = events,
                    Announcements = announcements
                };

                return View(viewModel); // Pass the ViewModel to the view
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while loading the dashboard.");
            }
        }
        //===============================================================================================
    }
}
//================================End==Of==File===========================================================
