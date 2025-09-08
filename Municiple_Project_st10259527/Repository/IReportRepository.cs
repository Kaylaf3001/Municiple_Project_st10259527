using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Repositories
{
    public interface IReportRepository
    {
        int GetTotalReports();
        IQueryable<ReportModel> GetRecentReports(int count = 5);
        IEnumerable<ReportModel> GetReportsByUserId(int userId, int? count = null);
        int GetReportsCountByUserId(int userId);
        void AddReport(ReportModel report, IFormFile? uploadedFile);
        
        // Report Retrieval
        Task<IEnumerable<ReportModel>> GetAllReportsAsync();
        Task<IEnumerable<ReportModel>> GetAllReportsWithUsersAsync();
        Task<ReportModel> GetReportByIdAsync(int id);
        Task<ReportModel> GetReportWithDetailsAsync(int id);
        
        // Status-based Retrieval
        Task<IEnumerable<ReportModel>> GetReportsByStatusAsync(ReportStatus status);
        Task<Dictionary<ReportStatus, int>> GetReportCountsByStatusAsync();
        
        // Report Updates
        Task UpdateReportStatusAsync(int reportId, ReportStatus status, string? adminNotes = null, int? assignedAdminId = null);
        Task UpdateReportAsync(ReportModel report);
        
        // Admin Actions
        Task<bool> ApproveReportAsync(int reportId, int adminId, string? notes = null);
        Task<bool> RejectReportAsync(int reportId, int adminId, string reason);
        Task<bool> MarkInReviewAsync(int reportId, int adminId);
    }
}
