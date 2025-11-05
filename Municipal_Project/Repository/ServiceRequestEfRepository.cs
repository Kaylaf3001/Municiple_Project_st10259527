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
        private readonly AppDbContext _db;
        public ServiceRequestEfRepository(AppDbContext db) { _db = db; }

        public async Task<ServiceRequestModel> AddAsync(ServiceRequestModel request)
        {
            _db.ServiceRequests.Add(request);
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task<ServiceRequestModel> GetByIdAsync(int id)
        {
            return await _db.ServiceRequests.FirstOrDefaultAsync(r => r.RequestId == id);
        }

        public async Task<ServiceRequestModel> GetByTrackingCodeAsync(string trackingCode)
        {
            return await _db.ServiceRequests.FirstOrDefaultAsync(r => r.TrackingCode == trackingCode);
        }

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

        public async Task UpdateStatusAsync(int id, ServiceRequestStatus status)
        {
            var entity = await _db.ServiceRequests.FirstOrDefaultAsync(r => r.RequestId == id);
            if (entity != null)
            {
                entity.Status = status;
                await _db.SaveChangesAsync();
            }
        }
    }
}
