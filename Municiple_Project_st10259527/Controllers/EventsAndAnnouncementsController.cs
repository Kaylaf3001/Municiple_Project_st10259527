using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;
using System;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Controllers
{
    public class EventsAndAnnouncementsController : Controller
    {
        private readonly EventManagementService _eventManagementService;
        private readonly IEventsRepository _eventsRepository;

        public EventsAndAnnouncementsController(
            EventManagementService eventManagementService,
            IEventsRepository eventsRepository)
        {
            _eventManagementService = eventManagementService;
            _eventsRepository = eventsRepository;
        }

        // GET: EventsAndAnnouncements/EventsDashboard
        public async Task<IActionResult> EventsDashboard(
            string selectedCategory = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string searchTerm = null)
        {
            try
            {
                var viewModel = await _eventManagementService.GetEventsDashboardViewModelAsync(
                    selectedCategory,
                    startDate,
                    endDate,
                    searchTerm);

                return View("EventsDashboard", viewModel);
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        // Record a recently viewed event
        [HttpGet]
        public async Task<IActionResult> ViewEvent(int id)
        {
            await _eventManagementService.LogEventViewAsync(id);
            return RedirectToAction(nameof(EventsDashboard));
        }

        // AJAX endpoint for logging views
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogView(int id)
        {
            try
            {
                var evt = await _eventsRepository.GetEventByIdAsync(id);
                if (evt == null)
                {
                    return NotFound();
                }

                await _eventManagementService.LogEventViewAsync(id);
                var recentEvents = await _eventManagementService.GetRecentlyViewedEventsAsync();

                return Json(new { success = true, recentEvents });
            }
            catch
            {
                return StatusCode(500, new { success = false, error = "An error occurred while updating recently viewed events." });
            }
        }

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

                ModelState.AddModelError("", "Unable to update event. Please try again.");
            }

            return View("EditEvent", eventModel);
        }

        // POST: Events/Delete/id
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
    }
}