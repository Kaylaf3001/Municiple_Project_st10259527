using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.ViewModels
{
    public class UserDashboardViewModel
    {
        // User Information
        public string UserName { get; set; }

        // Reports
        public int TotalReports { get; set; }
        public List<ReportModel> RecentReports { get; set; } = new List<ReportModel>();

        // Upcoming Events
        public EventModel NextUpcomingEvent { get; set; }
        public int QueueCount { get; set; }
        public bool HasEventsInQueue => QueueCount > 0;
    }
}