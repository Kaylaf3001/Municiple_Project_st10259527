using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Repository
{
    public interface IAdminRepository
    {
        // Dashboard
        Task<AdminDashboardViewModel> GetDashboardDataAsync();
        
        // Reports
        Task<ReportModel> GetReportByIdAsync(int id);
        Task UpdateReportStatusAsync(int reportId, ReportStatus status, string adminNotes = null);
        Task<IEnumerable<ReportModel>> GetReportsAsync(ReportStatus? status = null, int page = 1, int pageSize = 10, string searchTerm = null);
        Task<int> GetReportCountByStatusAsync(ReportStatus status);
        Task<int> GetTotalReportCountAsync();
        
        // Additional methods needed for AdminRepository
        Task<int> GetTotalUserCountAsync();
        Task<List<ReportModel>> GetRecentReportsByStatusAsync(ReportStatus status, int count);
    }
}
