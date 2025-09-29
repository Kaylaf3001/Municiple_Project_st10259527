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

        public void AddAnnouncement(AnnouncementModel announcement)
        {
            _context.Announcements.Add(announcement);
            _context.SaveChanges();
        }

        public IEnumerable<AnnouncementModel> GetAllAnnouncements()
        {
            return _context.Announcements.ToList();
        }
    }
}
