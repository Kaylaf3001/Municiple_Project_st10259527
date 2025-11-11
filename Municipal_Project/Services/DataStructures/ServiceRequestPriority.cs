using System;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.Services.DataStructures
{
    public class ServiceRequestPriority : IComparable<ServiceRequestPriority>
    {
        //=====================================================================
        // Properties
        //=====================================================================
        #region
        public int Priority { get; }
        public DateTime SubmittedAt { get; }
        public ServiceRequestModel Request { get; }  
        //=====================================================================

        //=====================================================================
        // Constructor
        //=====================================================================
        public ServiceRequestPriority(ServiceRequestModel request)
        {
            Priority = request.Priority;
            SubmittedAt = request.SubmittedAt;
            Request = request;
        }
        #endregion
        //=====================================================================

        //=====================================================================
        // Comparison Logic, this is used to order the requests by priority and
        // submission date.
        //=====================================================================
        public int CompareTo(ServiceRequestPriority other)
        {
            // First compare by priority (lower number = higher priority)
            int priorityComparison = Priority.CompareTo(other.Priority);
            
            // If priorities are equal, compare by submission date (earlier = higher priority)
            if (priorityComparison == 0)
            {
                return SubmittedAt.CompareTo(other.SubmittedAt);
            }
            
            return priorityComparison;
        }
        //=====================================================================
    }
}
//=================================End=Of=File=================================
