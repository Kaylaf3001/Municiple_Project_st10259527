using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;
using System;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Controllers
{
    public class EventsAndAnnouncementsController : Controller
    {
        //===================================================================================
        // Dependencies Injection
        //===================================================================================
        #region
        private readonly EventManagementService _eventManagementService;
        private readonly IEventsRepository _eventsRepository;
        private readonly IUserSearchHistoryRepository _searchRepository;
        private readonly IAnnouncementsRepository _announcementsRepository;



        public EventsAndAnnouncementsController(EventManagementService eventManagementService, IEventsRepository eventsRepository,IUserSearchHistoryRepository searchRepository, IAnnouncementsRepository announcementsRepository)
        {
            _eventManagementService = eventManagementService;
            _eventsRepository = eventsRepository;
            _searchRepository = searchRepository;
            _announcementsRepository = announcementsRepository;
        }
        #endregion
        //===================================================================================

        //===================================================================================
        // GET: EventsAndAnnouncements/EventsDashboard
        //===================================================================================
        public async Task<IActionResult> EventsDashboard(string selectedCategory = null,DateTime? startDate = null,DateTime? endDate = null,string searchTerm = null)
        {
            try
            {
                var sessionUserId = HttpContext.Session.GetInt32("UserId");
                if (sessionUserId.HasValue)
                {
                    if (!string.IsNullOrWhiteSpace(searchTerm) || !string.IsNullOrWhiteSpace(selectedCategory))
                    {
                        await _searchRepository.LogSearchAsync(sessionUserId.Value, searchTerm, selectedCategory ?? "General");
                    }
                }

                var viewModel = await _eventManagementService.GetEventsDashboardViewModelAsync(
                    selectedCategory,
                    startDate,
                    endDate,
                    searchTerm);

                viewModel.Announcements = await _announcementsRepository.GetAllAnnouncementsAsync();

                return View("EventsDashboard", viewModel);
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }
        //===================================================================================

        //===============================================================================
        // Record a recently viewed event
        //===============================================================================
        [HttpGet]
        public async Task<IActionResult> ViewEvent(int id)
        {
            await _eventManagementService.LogEventViewAsync(id);
            return RedirectToAction(nameof(EventsDashboard));
        }
        //===============================================================================

        //===============================================================================
        // Log a user search
        //===============================================================================
        public async Task LogSearchAsync(int userId, string searchTerm, string category)
        {
            var search = new UserSearchHistory
            {
                UserId = userId,
                SearchTerm = searchTerm,
                Category = category,
                SearchDate = DateTime.UtcNow
            };

            await _searchRepository.AddSearchAsync(search);
        }
        //===============================================================================

        //===============================================================================
        // GET: Events/Edit
        //===============================================================================
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
        //===============================================================================

        //===============================================================================
        // POST: Events/Edit
        //===============================================================================
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
        //===============================================================================

        //===============================================================================
        // POST: Events/Delete/id
        //===============================================================================
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
        //===============================================================================
    }
}
//=========================End==Of==File=================================================