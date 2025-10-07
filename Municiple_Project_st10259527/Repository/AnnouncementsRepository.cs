using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.Repository
{
    public class AnnouncementsRepository : IAnnouncementsRepository
    {
        private readonly AppDbContext _context;

        public AnnouncementsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAnnouncementAsync(AnnouncementModel announcement)
        {
            await _context.Announcements.AddAsync(announcement);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<AnnouncementModel> GetAllAnnouncements()
        {
            return _context.Announcements.ToList();
        }
    }
}
