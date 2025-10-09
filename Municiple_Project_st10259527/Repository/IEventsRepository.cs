using Microsoft.AspNetCore.Mvc;

namespace Municiple_Project_st10259527.Repository
{
    public interface IEventsRepository
    {
        Task AddEventAsync(Models.EventModel eventModel);
        Task<bool> UpdateEventAsync(Models.EventModel updatedEvent);
        Task<bool> DeleteEventAsync(int eventId);
        Task<IEnumerable<Models.EventModel>> GetAllEventsAsync();
        Queue<Models.EventModel> GetUpcomingEventsQueue();
        Task<Models.EventModel?> GetEventByIdAsync(int id);

    }
}
