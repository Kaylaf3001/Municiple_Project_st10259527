using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;
using System.Collections.Generic;
using System.Linq;

namespace Municiple_Project_st10259527.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public int GetTotalReports()
        {
            return _context.Reports.Count();
        }

        public List<ReportModel> GetReportsByUserId(int userId)
        {
            return _context.Reports
                           .Where(r => r.UserId == userId)
                           .OrderByDescending(r => r.ReportDate)
                           .ToList();
        }

        public List<ReportModel> GetRecentReports(int count = 5)
        {
            return _context.Reports
                           .OrderByDescending(r => r.ReportDate)
                           .Take(count)
                           .ToList();
        }
    }
}
