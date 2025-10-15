using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.Repository
{
    public interface IEventsRepository
    {
        // Basic CRUD operations
        Task AddEventAsync(EventModel eventModel);
        Task<bool> UpdateEventAsync(EventModel updatedEvent);
        Task<bool> DeleteEventAsync(int eventId);
        Task<IEnumerable<EventModel>> GetAllEventsAsync();
        Task<IEnumerable<string>> GetAllCategoriesAsync();
        Task<EventModel?> GetEventByIdAsync(int id);
        int GetEventsCount();

        // Queue operations
        Task<Queue<EventModel>> GetUpcomingEventsQueueAsync();
        Task<EventModel?> GetNextUpcomingEventAsync();
        Task<EventModel?> DequeueNextEventAsync();
    }
}
