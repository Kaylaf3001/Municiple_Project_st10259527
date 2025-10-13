using Microsoft.AspNetCore.Mvc;

namespace Municiple_Project_st10259527.Repository
{
    public interface IEventsRepository
    {
        // Basic CRUD operations
        Task AddEventAsync(Models.EventModel eventModel);
        Task<bool> UpdateEventAsync(Models.EventModel updatedEvent);
        Task<bool> DeleteEventAsync(int eventId);
        Task<IEnumerable<Models.EventModel>> GetAllEventsAsync();
        Task<IEnumerable<string>> GetAllCategoriesAsync();
        Task<Models.EventModel?> GetEventByIdAsync(int id);

        // Queue operations
        Task<Queue<Models.EventModel>> GetUpcomingEventsQueueAsync();
        Task<Models.EventModel?> GetNextUpcomingEventAsync();
        Task<Models.EventModel?> DequeueNextEventAsync();
    }
}
