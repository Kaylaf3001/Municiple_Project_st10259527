using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.Repository;

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
                //Goes to the repository to get all events from the database
                var events = await _eventsRepository.GetAllEventsAsync();
                return View(events);
            }
            catch (Exception ex)
            {
                // Loging any errors
                return StatusCode(500, "An error occurred while loading events.");
            }
        }
        //===============================================================================================
    }
}
//================================End==Of==File===========================================================
