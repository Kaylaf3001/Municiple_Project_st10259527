using Microsoft.AspNetCore.Http;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Repositories
{
    public interface IReportRepository
    {
        int GetTotalReports();
        List<ReportModel> GetRecentReports(int count = 5);
        List<ReportModel> GetReportsByUserId(int userId);

        // Update to match repository method
        void AddReport(ReportModel report, IFormFile? uploadedFile);
    }
}
