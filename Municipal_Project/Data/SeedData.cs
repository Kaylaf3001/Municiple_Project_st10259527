using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;
using System;
using System.Linq;

namespace Municiple_Project_st10259527.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                // Check if we already have users
                if (context.Users.Any())
                {
                    return; // DB has been seeded
                }

                // Create users
                var adminUser = new UserModel
                {
                    UserId = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@municipality.gov.za",
                    Password = "Admin@123", // In production, this should be hashed
                    IsAdmin = true
                };

                var regularUser1 = new UserModel
                {
                    UserId = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Password = "User@123" // In production, this should be hashed
                };

                var regularUser2 = new UserModel
                {
                    UserId = 3,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Password = "User@123", // In production, this should be hashed
                    IsAdmin = false
                };

                context.Users.AddRange(adminUser, regularUser1, regularUser2);

                // Create events
                var events = new[]
                {
                    // --- Existing entries above ---
                    new EventModel
                    {
                        EventId = 1,
                        Title = "Town Hall Meeting",
                        Location = "City Hall",
                        Date = DateTime.Now.AddDays(7),
                        Category = "Government",
                        Description = "Monthly town hall meeting to discuss community matters.",
                        Status = "Scheduled",
                        UserId = adminUser.UserId
                    },
                    new EventModel
                    {
                        EventId = 2,
                        Title = "Community Cleanup",
                        Location = "Riverside Park",
                        Date = DateTime.Now.AddDays(14),
                        Category = "Community",
                        Description = "Help clean up our local park and make it beautiful again!",
                        Status = "Scheduled",
                        UserId = regularUser1.UserId
                    },
                    new EventModel { EventId = 3, Title = "Fire Safety Workshop", Location = LocationService.WesternCapeLocations[14], Date = DateTime.Now.AddDays(21), Category = "Safety", Description = "Learn about fire prevention and safety.", Status = "Scheduled", UserId = adminUser.UserId },
                    new EventModel { EventId = 4, Title = "Youth Sports Day", Location = LocationService.WesternCapeLocations[22], Date = DateTime.Now.AddDays(28), Category = "Recreation", Description = "Annual sports event for local youth.", Status = "Scheduled", UserId = regularUser2.UserId },
                    new EventModel { EventId = 5, Title = "Senior Health Fair", Location = LocationService.WesternCapeLocations[35], Date = DateTime.Now.AddDays(10), Category = "Health", Description = "Free health screenings for seniors.", Status = "Scheduled", UserId = regularUser1.UserId }
                };

                context.Events.AddRange(events);

                // Create announcements
                var announcements = new[]
                {
                    // --- Existing entries above ---
                    new AnnouncementModel
                    {
                        AnnouncementId = 1,
                        Title = "Water Supply Maintenance",
                        Date = DateTime.Now,
                        Description = "Scheduled maintenance on the water supply system on 2025-12-01 from 9:00 AM to 3:00 PM.",
                        Location = "Downtown Area",
                        Status = "Published",
                        UserId = adminUser.UserId
                    },
                    new AnnouncementModel
                    {
                        AnnouncementId = 2,
                        Title = "Road Closure Notice",
                        Date = DateTime.Now.AddDays(1),
                        Description = "Main Street will be closed for repairs from 2025-12-05 to 2025-12-10.",
                        Location = "Main Street",
                        Status = "Published",
                        UserId = adminUser.UserId
                    },
                    new AnnouncementModel { AnnouncementId = 3, Title = "Library Renovation", Date = DateTime.Now.AddDays(3), Description = "The city library will be closed for renovations from 2025-12-10 to 2025-12-20.", Location = LocationService.WesternCapeLocations[9], Status = "Published", UserId = adminUser.UserId },
                    new AnnouncementModel { AnnouncementId = 4, Title = "New Recycling Program", Date = DateTime.Now.AddDays(5), Description = "A new recycling program will be launched next month.", Location = LocationService.WesternCapeLocations[40], Status = "Published", UserId = adminUser.UserId },
                    new AnnouncementModel { AnnouncementId = 5, Title = "Community Garden Opening", Date = DateTime.Now.AddDays(7), Description = "Join us for the opening of the new community garden.", Location = LocationService.WesternCapeLocations[60], Status = "Published", UserId = adminUser.UserId }
                };

                context.Announcements.AddRange(announcements);

                // Create service requests
                var serviceRequests = new[]
                {
                    // --- Existing entries above ---
                    // Road Maintenance
                    new ServiceRequestModel
                    {
                        RequestId = 1,
                        UserId = regularUser1.UserId,
                        Title = "Pothole Repair",
                        Description = "Large pothole causing damage to vehicles on the main road",
                        Location = LocationService.WesternCapeLocations[36],
                        Status = ServiceRequestStatus.Submitted,
                        Priority = 1,
                        Category = "Infrastructure",
                        SubmittedAt = DateTime.Now.AddDays(-10)
                    },
                    new ServiceRequestModel
                    {
                        RequestId = 2,
                        UserId = regularUser1.UserId,
                        Title = "Road Resurfacing",
                        Description = "Entire street needs resurfacing, multiple potholes and cracks",
                        Location = LocationService.WesternCapeLocations[31],
                        Status = ServiceRequestStatus.InProgress,
                        Priority = 2,
                        Category = "Infrastructure",
                        SubmittedAt = DateTime.Now.AddDays(-7)
                    },
                    
                    // Utilities
                    new ServiceRequestModel
                    {
                        RequestId = 3,
                        UserId = adminUser.UserId,
                        Title = "Street Light Out",
                        Description = "Street light not working, making the area unsafe at night",
                        Location = LocationService.WesternCapeLocations[30],
                        Status = ServiceRequestStatus.InProgress,
                        Priority = 1,
                        Category = "Utilities",
                        SubmittedAt = DateTime.Now.AddDays(-5)
                    },
                    new ServiceRequestModel
                    {
                        RequestId = 4,
                        UserId = regularUser1.UserId,
                        Title = "Water Leak",
                        Description = "Water leaking from fire hydrant, causing water wastage",
                        Location = LocationService.WesternCapeLocations[29],
                        Status = ServiceRequestStatus.Submitted,
                        Priority = 1,
                        Category = "Utilities",
                        SubmittedAt = DateTime.Now.AddDays(-2)
                    },

                    // Public Safety
                    new ServiceRequestModel
                    {
                        RequestId = 5,
                        UserId = adminUser.UserId,
                        Title = "Broken Playground Equipment",
                        Description = "Swing set is broken and poses safety hazard to children",
                        Location = LocationService.WesternCapeLocations[59],
                        Status = ServiceRequestStatus.Completed,
                        Priority = 2,
                        Category = "Safety",
                        SubmittedAt = DateTime.Now.AddDays(-14),
                        CompletedAt = DateTime.Now.AddDays(-7)
                    },
                    
                    // Sanitation
                    new ServiceRequestModel
                    {
                        RequestId = 6,
                        UserId = regularUser1.UserId,
                        Title = "Overflowing Trash Bin",
                        Description = "Public trash bin is overflowing and attracting pests",
                        Location = LocationService.WesternCapeLocations[41],
                        Status = ServiceRequestStatus.Submitted,
                        Priority = 3,
                        Category = "Waste",
                        SubmittedAt = DateTime.Now.AddDays(-1)
                    },
                    
                    // Parks and Recreation
                    new ServiceRequestModel
                    {
                        RequestId = 7,
                        UserId = adminUser.UserId,
                        Title = "Broken Park Bench",
                        Description = "Wooden slats on park bench are broken and need replacement",
                        Location = LocationService.WesternCapeLocations[60],
                        Status = ServiceRequestStatus.OnHold,
                        Priority = 3,
                        Category = "Parks",
                        SubmittedAt = DateTime.Now.AddDays(-21)
                    },
                    
                    // Traffic and Signs
                    new ServiceRequestModel
                    {
                        RequestId = 8,
                        UserId = regularUser1.UserId,
                        Title = "Missing Stop Sign",
                        Description = "Stop sign knocked down at intersection, creating dangerous situation",
                        Location = LocationService.WesternCapeLocations[42],
                        Status = ServiceRequestStatus.Submitted,
                        Priority = 1,
                        Category = "Safety",
                        SubmittedAt = DateTime.Now.AddDays(-3)
                    },
                    new ServiceRequestModel { RequestId = 9, UserId = regularUser2.UserId, Title = "Graffiti Removal", Description = "Graffiti on public wall", Location = LocationService.WesternCapeLocations[2], Status = ServiceRequestStatus.InProgress, Priority = 2, Category = "Sanitation", SubmittedAt = DateTime.Now.AddDays(-6) },
                    new ServiceRequestModel { RequestId = 10, UserId = regularUser1.UserId, Title = "Tree Trimming", Description = "Overgrown tree branches blocking streetlight", Location = LocationService.WesternCapeLocations[10], Status = ServiceRequestStatus.Submitted, Priority = 3, Category = "Parks", SubmittedAt = DateTime.Now.AddDays(-8) },
                    new ServiceRequestModel { RequestId = 11, UserId = regularUser2.UserId, Title = "Flooded Sidewalk", Description = "Sidewalk flooded after rain", Location = LocationService.WesternCapeLocations[20], Status = ServiceRequestStatus.OnHold, Priority = 2, Category = "Infrastructure", SubmittedAt = DateTime.Now.AddDays(-12) },
                    new ServiceRequestModel { RequestId = 12, UserId = regularUser1.UserId, Title = "Illegal Dumping", Description = "Trash dumped in vacant lot", Location = LocationService.WesternCapeLocations[50], Status = ServiceRequestStatus.Submitted, Priority = 2, Category = "Sanitation", SubmittedAt = DateTime.Now.AddDays(-2) },
                    new ServiceRequestModel { RequestId = 13, UserId = regularUser2.UserId, Title = "Broken Water Meter", Description = "Water meter cover missing", Location = LocationService.WesternCapeLocations[55], Status = ServiceRequestStatus.Completed, Priority = 1, Category = "Utilities", SubmittedAt = DateTime.Now.AddDays(-15), CompletedAt = DateTime.Now.AddDays(-10) },
                    new ServiceRequestModel { RequestId = 14, UserId = regularUser1.UserId, Title = "Blocked Drain", Description = "Drain blocked by leaves", Location = LocationService.WesternCapeLocations[65], Status = ServiceRequestStatus.Submitted, Priority = 3, Category = "Water/Sewer", SubmittedAt = DateTime.Now.AddDays(-4) },
                    new ServiceRequestModel { RequestId = 15, UserId = regularUser2.UserId, Title = "Playground Vandalism", Description = "Playground equipment damaged", Location = LocationService.WesternCapeLocations[71], Status = ServiceRequestStatus.InProgress, Priority = 1, Category = "Safety", SubmittedAt = DateTime.Now.AddDays(-7) }
                };

                context.ServiceRequests.AddRange(serviceRequests);

                // Create reports
                var reports = new[]
                {
                    // Infrastructure Reports
                    new ReportModel
                    {
                        ReportId = 1,
                        UserId = regularUser1.UserId,
                        ReportType = "Infrastructure",
                        Description = "Broken sidewalk causing accessibility issues for wheelchair users",
                        Location = "300 Block of Pine Street",
                        Status = ReportStatus.Pending,
                        ReportDate = DateTime.Now.AddDays(-14),
                        AdminNotes = "Needs assessment for ADA compliance"
                    },
                    new ReportModel
                    {
                        ReportId = 2,
                        UserId = regularUser1.UserId,
                        ReportType = "Infrastructure",
                        Description = "Damaged guardrail along the bridge",
                        Location = "Riverside Drive Bridge",
                        Status = ReportStatus.InReview,
                        ReportDate = DateTime.Now.AddDays(-7),
                        AssignedAdminId = adminUser.UserId,
                        AdminNotes = "Safety issue, needs immediate attention"
                    },
                    
                    // Public Safety Reports
                    new ReportModel
                    {
                        ReportId = 3,
                        UserId = regularUser2.UserId,
                        ReportType = "Public Safety",
                        Description = "Graffiti on public property",
                        Location = "Community Center Back Wall",
                        Status = ReportStatus.InReview,
                        ReportDate = DateTime.Now.AddDays(-5),
                        AssignedAdminId = adminUser.UserId
                    },
                    new ReportModel
                    {
                        ReportId = 4,
                        UserId = regularUser1.UserId,
                        ReportType = "Public Safety",
                        Description = "Suspicious activity in the park after hours",
                        Location = "Central Park, near the playground",
                        Status = ReportStatus.Approved,
                        ReportDate = DateTime.Now.AddDays(-10),
                        AssignedAdminId = adminUser.UserId,
                        AdminNotes = "Increased patrols in the area"
                    },
                    
                    // Environmental Issues
                    new ReportModel
                    {
                        ReportId = 5,
                        UserId = regularUser2.UserId,
                        ReportType = "Environmental",
                        Description = "Illegal dumping in the vacant lot",
                        Location = "Behind 450 Oak Street",
                        Status = ReportStatus.Pending,
                        ReportDate = DateTime.Now.AddDays(-3)
                    },
                    
                    // Traffic and Transportation
                    new ReportModel
                    {
                        ReportId = 6,
                        UserId = regularUser1.UserId,
                        ReportType = "Traffic",
                        Description = "Traffic light timing issue causing congestion during rush hour",
                        Location = "Intersection of Main and 5th",
                        Status = ReportStatus.Completed,
                        ReportDate = DateTime.Now.AddDays(-21),
                        AdminNotes = "Timing adjusted, monitoring traffic flow"
                    },
                    
                    // Parks and Recreation
                    new ReportModel
                    {
                        ReportId = 7,
                        UserId = regularUser2.UserId,
                        ReportType = "Parks",
                        Description = "Fallen tree blocking walking trail",
                        Location = "Riverside Park, North Trail",
                        Status = ReportStatus.InReview,
                        ReportDate = DateTime.Now.AddDays(-2),
                        AssignedAdminId = adminUser.UserId
                    },
                    
                    // Noise Complaint
                    new ReportModel
                    {
                        ReportId = 8,
                        UserId = regularUser1.UserId,
                        ReportType = "Noise",
                        Description = "Excessive noise from construction work outside permitted hours",
                        Location = "Construction site at 200 Maple Street",
                        Status = ReportStatus.Pending,
                        ReportDate = DateTime.Now.AddDays(-1)
                    },
                    
                    // Animal Control
                    new ReportModel
                    {
                        ReportId = 9,
                        UserId = regularUser2.UserId,
                        ReportType = "Animal Control",
                        Description = "Aggressive stray dog in the neighborhood",
                        Location = "400 Block of Walnut Street",
                        Status = ReportStatus.Completed,
                        ReportDate = DateTime.Now.AddDays(-5),
                        AdminNotes = "Animal control notified, dog captured and taken to shelter"
                    },
                    
                    // Water and Sewer
                    new ReportModel
                    {
                        ReportId = 10,
                        UserId = regularUser1.UserId,
                        ReportType = "Water/Sewer",
                        Description = "Sewer drain is clogged and causing water to pool",
                        Location = "Corner of 2nd and Elm",
                        Status = ReportStatus.InReview,
                        ReportDate = DateTime.Now.AddDays(-3),
                        AssignedAdminId = adminUser.UserId
                    }
                };

                context.Reports.AddRange(reports);

                // Save all changes
                context.SaveChanges();
            }
        }
    }
}
