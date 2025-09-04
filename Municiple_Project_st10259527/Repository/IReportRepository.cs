using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;
using System.Linq;

namespace Municiple_Project_st10259527.Repositories
{
    public interface IReportRepository
    {
        int GetTotalReports();
        IQueryable<ReportModel> GetRecentReports(int count = 5);
        IEnumerable<ReportModel> GetReportsByUserId(int userId, int? count = null);
        int GetReportsCountByUserId(int userId);
        void AddReport(ReportModel report, IFormFile? uploadedFile);
    }
}
