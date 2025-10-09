using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;

namespace Municiple_Project_st10259527.Repository
{
    public class EventsRepository : IEventsRepository
    {
        private readonly AppDbContext _context;

        public EventsRepository(AppDbContext context)
        {
            _context = context;
        }

        //CRUD

        //===============================================================================================
        //Create Event
        //===============================================================================================
        public async Task AddEventAsync(EventModel eventModel)
        {
            await _context.Events.AddAsync(eventModel);
            await _context.SaveChangesAsync();
        }
        //===============================================================================================

        //===============================================================================================
        //Delete Event by Id
        //===============================================================================================
        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var eventToDelete = await _context.Events.FindAsync(eventId);
            if (eventToDelete == null)
            {
                return false; // Event not found
            }
            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
            return true; // Deletion successful

        }
        //===============================================================================================

        //===============================================================================================
        //Update Event
        //===============================================================================================
        public async Task<bool> UpdateEventAsync(EventModel updatedEvent)
        {
            try
            {
                var existingEvent = await _context.Events.FindAsync(updatedEvent.EventId);
                if (existingEvent == null)
                {
                    return false; // Event not found
                }

                // Update only the properties that should be updatable
                existingEvent.Title = updatedEvent.Title;
                existingEvent.Location = updatedEvent.Location;
                existingEvent.Date = updatedEvent.Date;
                existingEvent.Category = updatedEvent.Category;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.Status = updatedEvent.Status;

                _context.Events.Update(existingEvent);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Log the error
                return false;
            }
        }
        //===============================================================================================

        //===============================================================================================
        //Get All Events
        //===============================================================================================
        public async Task<IEnumerable<EventModel>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }
        //===============================================================================================

        //===============================================================================================
        //Get Upcoming Events in a Queue
        //===============================================================================================
        public Queue<EventModel> GetUpcomingEventsQueue()
        {
            var currentDate = DateTime.Now;

            // Step 1: Fetch the upcoming events from the database
            var upcomingEvents = _context.Events
                .Where(e => e.Date >= currentDate)
                .OrderBy(e => e.Date)
                .ToList();

            // Step 2: Create a queue and enqueue each event
            Queue<EventModel> eventQueue = new Queue<EventModel>();

            foreach (var ev in upcomingEvents)
            {
                eventQueue.Enqueue(ev);
            }

            return eventQueue;
        }
        //===============================================================================================

        //===============================================================================================
        //Get Event by Id
        //===============================================================================================
        public async Task<EventModel> GetEventByIdAsync(int id)
        {
            return await _context.Events.Include(e => e.User).FirstOrDefaultAsync(e => e.EventId == id);
        }
        //===============================================================================================

    }
}
