using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.Repository
{
    public interface IAnnouncementsRepository
    {
        Task AddAnnouncementAsync(AnnouncementModel announcement);
        IEnumerable<AnnouncementModel> GetAllAnnouncements();
    }
}
