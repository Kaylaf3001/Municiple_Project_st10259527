using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.ViewModels;

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

        public EventsAndAnnouncementsController(IEventsRepository eventsRepository, IAnnouncementsRepository announcementsRepository, EventAnnouncementService eventAnnouncementService)
        {
            _eventsRepository = eventsRepository;
            _announcementsRepository = announcementsRepository;
            _eventAnnouncementService = eventAnnouncementService;
        }
        #endregion
        //===============================================================================================

        //===============================================================================================
        // GET: EventsAndAnnouncements/EventsDashboard
        // Handles both regular view and filtering
        //===============================================================================================
        [HttpGet]
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
                
                // Get unique categories for the filter dropdown
                var categories = allEvents
                    .Select(e => e.Category)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                // Get the next upcoming event (from unfiltered data)
                var nextEvent = await _eventsRepository.GetNextUpcomingEventAsync();
                
                // Get queue count
                var upcomingEventsQueue = await _eventsRepository.GetUpcomingEventsQueueAsync();
                
                // Check if any filter parameters are provided
                bool hasFilters = !string.IsNullOrEmpty(selectedCategory) || 
                                startDate.HasValue || 
                                endDate.HasValue || 
                                !string.IsNullOrEmpty(searchTerm);
                
                // Initialize ViewModel with unfiltered data by default
                var viewModel = new EventsAndAnnouncementsViewModel
                {
                    Events = allEvents,
                    Announcements = allAnnouncements,
                    NextUpcomingEvent = nextEvent,
                    QueueCount = upcomingEventsQueue.Count,
                    Categories = categories,
                    SelectedCategory = selectedCategory,
                    StartDate = startDate,
                    EndDate = endDate,
                    SearchTerm = searchTerm
                };

                // Apply filters if any filter parameters are provided
                if (hasFilters)
                {
                    _eventAnnouncementService.Initialize(allEvents, allAnnouncements);
                    var (filteredEvents, filteredAnnouncements) = _eventAnnouncementService.Filter(new EventAnnouncementFilter
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        Category = selectedCategory,
                        SearchTerm = searchTerm
                    });

                    viewModel.Events = filteredEvents.ToList();
                    viewModel.Announcements = filteredAnnouncements.ToList();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "An error occurred while loading the dashboard.");
                return View(new EventsAndAnnouncementsViewModel());
            }
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
//================================End==Of==File===========================================================
