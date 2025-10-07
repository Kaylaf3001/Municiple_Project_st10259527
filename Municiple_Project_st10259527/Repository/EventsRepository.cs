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

        public async Task AddEventAsync(EventModel eventModel)
        {
            await _context.Events.AddAsync(eventModel);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EventModel>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

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


    }
}
