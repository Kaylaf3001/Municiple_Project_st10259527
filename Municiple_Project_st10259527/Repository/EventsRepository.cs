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

        public void AddEvent(EventModel eventModel)
        {
            _context.Events.Add(eventModel);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<EventModel>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

    }
}
