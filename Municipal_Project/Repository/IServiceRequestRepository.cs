using System.Collections.Generic;
using System.Threading.Tasks;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.Repository
{
    //=================================================================
    // Service Request Repository Interface
    //=================================================================
    public interface IServiceRequestRepository
    {
        Task<ServiceRequestModel> AddAsync(ServiceRequestModel request);
        Task<ServiceRequestModel> GetByIdAsync(int id);
        Task<ServiceRequestModel> GetByTrackingCodeAsync(string trackingCode);
        IAsyncEnumerable<ServiceRequestModel> GetByUserAsync(int userId);
        IAsyncEnumerable<ServiceRequestModel> GetAllAsync();
        IAsyncEnumerable<ServiceRequestModel> GetByStatusAsync(ServiceRequestStatus status);
        Task UpdateStatusAsync(int id, ServiceRequestStatus status);
    }
    //=================================================================
}
//==================================End=Of=File==========================================================
