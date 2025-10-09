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

        public EventsAndAnnouncementsController(IEventsRepository eventsRepository, IAnnouncementsRepository announcementsRepository)
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
                // Get events, announcements, and upcoming events queue
                var events = await _eventsRepository.GetAllEventsAsync();
                var announcements = await _announcementsRepository.GetAllAnnouncementsAsync();
                var upcomingEventsQueue = _eventsRepository.GetUpcomingEventsQueue();

                // Create and populate the ViewModel
                var viewModel = new EventsAndAnnouncementsViewModel
                {
                    Events = events,
                    Announcements = announcements,
                    UpcomingEventsQueue = upcomingEventsQueue
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while loading the dashboard.");
            }
        }
        //===============================================================================================

        // GET: Events/Edit
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
            return RedirectToAction("ManageEvents","Events");
        }
    }
}
//================================End==Of==File===========================================================
