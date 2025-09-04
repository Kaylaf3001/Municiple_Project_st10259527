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
            Console.WriteLine("[DEBUG] ===== Starting AddReport =====");
            
            if (report == null)
            {
                Console.WriteLine("[ERROR] Report is null");
                throw new ArgumentNullException(nameof(report), "Report cannot be null");
            }

            // Log incoming report data
            Console.WriteLine($"[DEBUG] Incoming Report Data - " +
                           $"UserId: {report.UserId}, " +
                           $"Type: {report.ReportType ?? "null"}, " +
                           $"Status: {report.Status ?? "null"}, " +
                           $"Location: {report.Location ?? "null"}, " +
                           $"Description: {report.Description ?? "null"}");

            try
            {
                // Set default values
                report.ReportDate = DateTime.Now;
                report.Status = string.IsNullOrWhiteSpace(report.Status) ? "Pending" : report.Status.Trim();
                report.ReportType = string.IsNullOrWhiteSpace(report.ReportType) ? "General" : report.ReportType.Trim();
                report.Description = string.IsNullOrWhiteSpace(report.Description) ? "No description provided" : report.Description.Trim();
                report.Location = string.IsNullOrWhiteSpace(report.Location) ? "Location not specified" : report.Location.Trim();
                
                // Log the values being saved
                Console.WriteLine($"[DEBUG] Saving Report - " +
                               $"UserId: {report.UserId}, " +
                               $"Type: {report.ReportType}, " +
                               $"Status: {report.Status}, " +
                               $"Location: {report.Location}, " +
                               $"Description: {report.Description}");

                // Handle file upload if present
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Console.WriteLine($"[DEBUG] Creating uploads directory: {uploadsFolder}");
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
                        Console.WriteLine($"[DEBUG] File uploaded successfully: {report.FilePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WARNING] Error uploading file: {ex.Message}");
                        Console.WriteLine($"[WARNING] Stack trace: {ex.StackTrace}");
                        report.FilePath = null;
                    }
                }
                else
                {
                    Console.WriteLine("[DEBUG] No file was uploaded");
                    report.FilePath = null;
                }

                // Add and save the report
                Console.WriteLine("[DEBUG] Adding report to database...");
                _context.Reports.Add(report);
                
                // Log the SQL that will be executed
                Console.WriteLine("[DEBUG] Saving changes to database...");
                int changes = _context.SaveChanges();
                
                if (changes > 0)
                {
                    Console.WriteLine($"[SUCCESS] Report saved successfully! ID: {report.ReportId}");
                }
                else
                {
                    Console.WriteLine("[WARNING] No changes were saved to the database");
                }
                
                // Verify the report was saved
                var savedReport = _context.Reports.Find(report.ReportId);
                if (savedReport != null)
                {
                    Console.WriteLine($"[DEBUG] Report verified in database - " +
                                   $"ID: {savedReport.ReportId}, " +
                                   $"Type: {savedReport.ReportType}, " +
                                   $"Status: {savedReport.Status}");
                }
                else
                {
                    Console.WriteLine("[ERROR] Failed to verify report in database after save");
                }
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"[ERROR] Database error while saving report: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner exception: {dbEx.InnerException.Message}");
                    if (dbEx.InnerException.InnerException != null)
                    {
                        Console.WriteLine($"[ERROR] Inner inner exception: {dbEx.InnerException.InnerException.Message}");
                    }
                }
                throw new Exception("Failed to save report to database. Please try again.", dbEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected error in AddReport: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                throw new Exception("An unexpected error occurred while saving the report.", ex);
            }
            finally
            {
                Console.WriteLine("[DEBUG] ===== End AddReport =====");
            }
        }

    }
}
