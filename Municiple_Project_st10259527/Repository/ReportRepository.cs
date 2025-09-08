using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; // Add this for DbUpdateException
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
            // Execute count directly in database
            return _context.Reports.Count();
        }

        public int GetReportsCountByUserId(int userId)
        {
            // Execute count directly in database
            return _context.Reports.Count(r => r.UserId == userId);
        }

        public IEnumerable<ReportModel> GetReportsByUserId(int userId, int? count = null)
        {
            // Create base query
            var query = _context.Reports
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReportDate);

            // Apply count if specified
            if (count.HasValue)
            {
                query = (IOrderedQueryable<ReportModel>)query.Take(count.Value);
            }

            // Execute query and return materialized results
            return query.ToList().AsEnumerable();
        }

        public async Task<IEnumerable<ReportModel>> GetAllReportsAsync()
        {
            return await _context.Reports
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReportModel>> GetAllReportsWithUsersAsync()
        {
            return await _context.Reports
                .Include(r => r.User)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }

        public async Task<ReportModel> GetReportByIdAsync(int id)
        {
            return await _context.Reports
                .FirstOrDefaultAsync(r => r.ReportId == id);
        }

        public async Task<ReportModel> GetReportWithDetailsAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReportId == id);
        }

        public async Task UpdateReportAsync(ReportModel report)
        {
            _context.Entry(report).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReportStatusAsync(int reportId, ReportStatus status, string? adminNotes = null, int? assignedAdminId = null)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                throw new KeyNotFoundException($"Report with ID {reportId} not found.");

            report.Status = status;
            report.LastUpdated = DateTime.UtcNow;
            
            if (!string.IsNullOrEmpty(adminNotes))
                report.AdminNotes = adminNotes;
                
            if (assignedAdminId.HasValue)
                report.AssignedAdminId = assignedAdminId;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ApproveReportAsync(int reportId, int adminId, string? notes = null)
        {
            try
            {
                await UpdateReportStatusAsync(reportId, ReportStatus.Approved, notes, adminId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RejectReportAsync(int reportId, int adminId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason is required.");

            try
            {
                await UpdateReportStatusAsync(reportId, ReportStatus.Rejected, $"Rejected: {reason}", adminId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkInReviewAsync(int reportId, int adminId)
        {
            try
            {
                await UpdateReportStatusAsync(reportId, ReportStatus.InReview, null, adminId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<ReportModel>> GetReportsByStatusAsync(ReportStatus status)
        {
            return await _context.Reports
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }

        public async Task<Dictionary<ReportStatus, int>> GetReportCountsByStatusAsync()
        {
            return await _context.Reports
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public IQueryable<ReportModel> GetRecentReports(int count = 5)
        {
            try
            {
                Console.WriteLine("[DEBUG] Getting recent reports...");
                
                // Return query without materializing it
                var query = _context.Reports
                    .OrderByDescending(r => r.ReportDate)
                    .Take(count)
                    .Select(r => new ReportModel
                    {
                        ReportId = r.ReportId,
                        UserId = r.UserId,
                        ReportType = r.ReportType,
                        Description = r.Description,
                        ReportDate = r.ReportDate,
                        Location = r.Location,
                        Status = r.Status,
                        FilePath = r.FilePath
                    });
                
                // Debug information - this will execute the query
                var debugCount = query.Count();
                Console.WriteLine($"[DEBUG] Query will return {debugCount} reports");
                
                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error in GetRecentReports: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                // Return an empty IQueryable by creating a new query that returns no results
                return _context.Reports.Take(0).AsQueryable();
            }
        }

        public void AddReport(ReportModel report, IFormFile? uploadedFile)
        {
            
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "Report cannot be null");
            }
            
            try
            {
                // Set default values
                report.ReportDate = DateTime.Now;
                // Status is now an enum, so we don't need to trim it
                if (report.Status == default)
                {
                    report.Status = ReportStatus.Pending;
                }
                report.ReportType = string.IsNullOrWhiteSpace(report.ReportType) ? "General" : report.ReportType.Trim();
                report.Description = string.IsNullOrWhiteSpace(report.Description) ? "No description provided" : report.Description.Trim();
                report.Location = string.IsNullOrWhiteSpace(report.Location) ? "Location not specified" : report.Location.Trim();
                
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var safeFileName = Path.GetFileName(uploadedFile.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(safeFileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        Console.WriteLine($"[DEBUG] Saving file to: {filePath}");
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            uploadedFile.CopyTo(stream);
                        }

                        report.FilePath = $"/uploads/{uniqueFileName}";
                    }
                    catch (Exception ex)
                    {
                        report.FilePath = null;
                    }
                }
                else
                {
                    report.FilePath = null;
                }

                // Add and save the report
                _context.Reports.Add(report);
                
                // Log the SQL that will be executed
                int changes = _context.SaveChanges();         
                
                // Verify the report was saved
                var savedReport = _context.Reports.Find(report.ReportId);
                
            }

            catch (DbUpdateException dbEx)
            {
                
                throw new Exception("Failed to save report to database. Please try again.", dbEx);
            }
           
        }

    }
}
