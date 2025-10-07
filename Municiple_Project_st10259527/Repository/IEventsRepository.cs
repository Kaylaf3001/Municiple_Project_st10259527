using Microsoft.AspNetCore.Mvc;

namespace Municiple_Project_st10259527.Repository
{
    public interface IEventsRepository
    {
        Task AddEventAsync(Models.EventModel eventModel);
        Task<IEnumerable<Models.EventModel>> GetAllEventsAsync();

    }
}
