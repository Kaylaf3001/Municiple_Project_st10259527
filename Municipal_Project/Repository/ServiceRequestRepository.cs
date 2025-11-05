using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.Repository
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        class Node
        {
            public ServiceRequestModel Value;
            public Node Next;
        }

        static Node head;
        static int nextId = 1;
        static readonly object sync = new object();

        public async Task<ServiceRequestModel> AddAsync(ServiceRequestModel request)
        {
            return await Task.Run(() =>
            {
                lock (sync)
                {
                    request.RequestId = nextId++;
                    var node = new Node { Value = request };
                    if (head == null) head = node;
                    else
                    {
                        var cur = head;
                        while (cur.Next != null) cur = cur.Next;
                        cur.Next = node;
                    }
                    return request;
                }
            });
        }

        public async Task<ServiceRequestModel> GetByIdAsync(int id)
        {
            return await Task.Run(() =>
            {
                var cur = head; while (cur != null) { if (cur.Value.RequestId == id) return cur.Value; cur = cur.Next; } return null;
            });
        }

        public async Task<ServiceRequestModel> GetByTrackingCodeAsync(string trackingCode)
        {
            return await Task.Run(() =>
            {
                var cur = head; while (cur != null) { if (cur.Value.TrackingCode == trackingCode) return cur.Value; cur = cur.Next; } return null;
            });
        }

        public async IAsyncEnumerable<ServiceRequestModel> GetByUserAsync(int userId)
        {
            // Seed if user has no requests
            if (!HasAnyForUser(userId))
            {
                await AddAsync(new ServiceRequestModel { UserId = userId, Title = "Water Leak Repair", Description = "Leak on Main St.", Priority = 1 });
                await AddAsync(new ServiceRequestModel { UserId = userId, Title = "Streetlight Outage", Description = "Near park entrance", Priority = 2, Status = ServiceRequestStatus.InProgress });
            }

            var cur = head;
            while (cur != null)
            {
                if (cur.Value.UserId == userId)
                    yield return cur.Value;
                cur = cur.Next;
            }
        }

        public async IAsyncEnumerable<ServiceRequestModel> GetAllAsync()
        {
            var cur = head;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Next;
            }
        }

        public async IAsyncEnumerable<ServiceRequestModel> GetByStatusAsync(ServiceRequestStatus status)
        {
            var cur = head;
            while (cur != null)
            {
                if (cur.Value.Status == status)
                    yield return cur.Value;
                cur = cur.Next;
            }
        }

        public async Task UpdateStatusAsync(int id, ServiceRequestStatus status)
        {
            await Task.Run(() =>
            {
                var cur = head; while (cur != null) { if (cur.Value.RequestId == id) { cur.Value.Status = status; break; } cur = cur.Next; }
            });
        }

        private bool HasAnyForUser(int userId)
        {
            var cur = head; while (cur != null) { if (cur.Value.UserId == userId) return true; cur = cur.Next; } return false;
        }
    }
}
