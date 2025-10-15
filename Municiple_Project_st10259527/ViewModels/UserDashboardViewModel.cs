using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;
using System.Linq;

namespace Municiple_Project_st10259527.ViewModels
{
    public class UserDashboardViewModel
    {
        // User Information
        public string UserName { get; set; }

        // Reports
        public int TotalReports { get; set; }
        public IEnumerable<ReportModel> RecentReports { get; set; } = Enumerable.Empty<ReportModel>();

        // Upcoming Events
        public EventModel NextUpcomingEvent { get; set; }
        public int QueueCount { get; set; }
        public bool HasEventsInQueue => QueueCount > 0;
        public int TotalEvents { get; set; }
    }
}