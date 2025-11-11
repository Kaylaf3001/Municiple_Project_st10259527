using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;

namespace Municiple_Project_st10259527.Repository
{
    public class ServiceRequestEfRepository : IServiceRequestRepository
    {
        //==============================================================================================
        // Dependency Injection
        //==============================================================================================
        #region
        private readonly AppDbContext _db;
        public ServiceRequestEfRepository(AppDbContext db) { _db = db; }

        public async Task<ServiceRequestModel> AddAsync(ServiceRequestModel request)
        {
            _db.ServiceRequests.Add(request);
            await _db.SaveChangesAsync();
            return request;
        }
        #endregion
        //==============================================================================================

        //==============================================================================================
        // Get by Id
        //==============================================================================================
        public async Task<ServiceRequestModel> GetByIdAsync(int id)
        {
            return await _db.ServiceRequests.FirstOrDefaultAsync(r => r.RequestId == id);
        }
        //==============================================================================================

        //==============================================================================================
        // Get by Tracking Code
        //==============================================================================================
        public async Task<ServiceRequestModel> GetByTrackingCodeAsync(string trackingCode)
        {
            return await _db.ServiceRequests.FirstOrDefaultAsync(r => r.TrackingCode == trackingCode);
        }
        //==============================================================================================

        //==============================================================================================
        // Get Service Requests by user id in descending order of submission date
        //==============================================================================================
        public async IAsyncEnumerable<ServiceRequestModel> GetByUserAsync(int userId)
        {
            await foreach (var r in _db.ServiceRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.SubmittedAt)
                .AsAsyncEnumerable()
                .WithCancellation(default))
            {
                yield return r;
            }
        }
        //==============================================================================================

        //==============================================================================================
        // Get All Service Requests in descending order of submission date
        //==============================================================================================
        public async IAsyncEnumerable<ServiceRequestModel> GetAllAsync()
        {
            await foreach (var r in _db.ServiceRequests
                .OrderByDescending(r => r.SubmittedAt)
                .AsAsyncEnumerable()
                .WithCancellation(default))
            {
                yield return r;
            }
        }
        //==============================================================================================

        //==============================================================================================
        // Get Service Requests by status in ascending order of priority
        //==============================================================================================
        public async IAsyncEnumerable<ServiceRequestModel> GetByStatusAsync(ServiceRequestStatus status)
        {
            await foreach (var r in _db.ServiceRequests
                .Where(r => r.Status == status)
                .OrderBy(r => r.Priority)
                .AsAsyncEnumerable()
                .WithCancellation(default))
            {
                yield return r;
            }
        }
        //==============================================================================================

        //==============================================================================================
        // Update Service Request Status by Id
        //==============================================================================================
        public async Task UpdateStatusAsync(int id, ServiceRequestStatus status)
        {
            var entity = await _db.ServiceRequests.FirstOrDefaultAsync(r => r.RequestId == id);
            if (entity != null)
            {
                entity.Status = status;
                await _db.SaveChangesAsync();
            }
        }
        //==============================================================================================

        //==============================================================================================
        // Update Service Request Status and Priority by Id
        //==============================================================================================
        public async Task UpdateStatusAndPriorityAsync(int id, ServiceRequestStatus status, int priority)
        {
            var entity = await _db.ServiceRequests.FirstOrDefaultAsync(r => r.RequestId == id);
            if (entity != null)
            {
                entity.Status = status;
                entity.Priority = priority;
                await _db.SaveChangesAsync();
            }
        }
        //==============================================================================================

        //==============================================================================================
        // Update a service request
        //==============================================================================================
        public async Task UpdateAsync(ServiceRequestModel request)
        {
            var existingRequest = await _db.ServiceRequests.FindAsync(request.RequestId);
            if (existingRequest != null)
            {
                // Update all properties except RequestId and TrackingCode
                var trackingCode = existingRequest.TrackingCode;
                var submittedAt = existingRequest.SubmittedAt;
                
                // Copy all properties from the updated request
                _db.Entry(existingRequest).CurrentValues.SetValues(request);
                
                // Preserve the original tracking code and submission date
                existingRequest.TrackingCode = trackingCode;
                existingRequest.SubmittedAt = submittedAt;
                
                // If the request is being marked as completed, set the completion time
                if (request.Status == ServiceRequestStatus.Completed && existingRequest.CompletedAt == null)
                {
                    existingRequest.CompletedAt = DateTime.UtcNow;
                }
                
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Service request with ID {request.RequestId} not found");
            }
        }
        //==============================================================================================
    }
    //==============================================================================================
}
//==================================End=Of=File==========================================================