using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;

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

        public async Task<IEnumerable<AnnouncementModel>> GetAllAnnouncementsAsync()
        {
            return await _context.Announcements.ToListAsync();
        }
    }
}
