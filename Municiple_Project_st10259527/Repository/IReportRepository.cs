using Municiple_Project_st10259527.Models;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Repositories
{
    public interface IReportRepository
    {
        // Total number of reports (for dashboard card)
        int GetTotalReports();

        // Get recent reports for all users (or can be filtered by user)
        List<ReportModel> GetRecentReports(int count = 5);

        // Get reports by specific user
        List<ReportModel> GetReportsByUserId(int userId);
    }
}
