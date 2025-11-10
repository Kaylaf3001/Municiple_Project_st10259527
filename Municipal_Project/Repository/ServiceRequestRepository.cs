using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.Repository
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        //==============================================================================================
        // In-Memory Linked List Storage
        // In this implementation, we use a simple linked list to store service requests in memory.
        //==============================================================================================
        class Node
        {
            public ServiceRequestModel Value;
            public Node Next;
        }

        static Node head;
        static int nextId = 1;
        static readonly object sync = new object();
        //==============================================================================================

        //==============================================================================================
        // Add a new service request
        // This method assigns a unique RequestId and appends the request to the linked list.
        //==============================================================================================
        public async Task<ServiceRequestModel> AddAsync(ServiceRequestModel request)
        {
            return await Task.Run(() =>
            {
                // Assign a unique RequestId and add to linked list
                lock (sync)
                {
                    // Generate a simple tracking code
                    request.RequestId = nextId++;
                    var node = new Node { Value = request };

                    // Append to linked list
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
        //==============================================================================================

        //==============================================================================================
        // Retrieve service requests based on various criteria
        //==============================================================================================
        public async Task<ServiceRequestModel> GetByIdAsync(int id)
        {
            // Traverse linked list to find request by ID
            return await Task.Run(() =>
            {
                var cur = head; while (cur != null) { if (cur.Value.RequestId == id) return cur.Value; cur = cur.Next; }
                return null;
            });
        }
        //==============================================================================================

        //==============================================================================================
        // Retrieve service request by tracking code
        //==============================================================================================
        public async Task<ServiceRequestModel> GetByTrackingCodeAsync(string trackingCode)
        {
            return await Task.Run(() =>
            {
                var cur = head; while (cur != null) { if (cur.Value.TrackingCode == trackingCode) return cur.Value; cur = cur.Next; }
                return null;
            });
        }
        //==============================================================================================

        //==============================================================================================
        // Retrieve service requests for a specific user
        // If the user has no requests, seed with default requests
        //==============================================================================================
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
        //==============================================================================================

        //==============================================================================================
        // Retrieve all service requests
        //==============================================================================================
        public async IAsyncEnumerable<ServiceRequestModel> GetAllAsync()
        {
            // Traverse linked list and yield all requests
            var cur = head;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Next;
            }
        }
        //==============================================================================================

        //==============================================================================================
        // Retrieve service requests by status
        //==============================================================================================
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
        //==============================================================================================

        //==============================================================================================
        // Update the status of a service request
        //==============================================================================================
        public async Task UpdateStatusAsync(int id, ServiceRequestStatus status)
        {
            await Task.Run(() =>
            {
                var cur = head; while (cur != null) { if (cur.Value.RequestId == id) { cur.Value.Status = status; break; } cur = cur.Next; }
            });
        }

        //==============================================================================================
        // Update the status and priority of a service request
        // Implementing interface member: Task UpdateStatusAndPriorityAsync(int id, ServiceRequestStatus status, int priority)
        //==============================================================================================
        public async Task UpdateStatusAndPriorityAsync(int id, ServiceRequestStatus status, int priority)
        {
            await Task.Run(() =>
            {
                var cur = head;
                while (cur != null)
                {
                    if (cur.Value.RequestId == id)
                    {
                        cur.Value.Status = status;
                        cur.Value.Priority = priority;
                        break;
                    }
                    cur = cur.Next;
                }
            });
        }
        //==============================================================================================

        //==============================================================================================
        // Helper method to check if any requests exist for a user
        //==============================================================================================
        private bool HasAnyForUser(int userId)
        {
            var cur = head; while (cur != null) { if (cur.Value.UserId == userId) return true; cur = cur.Next; }
            return false;
        }
        //==============================================================================================
    }
}
//==================================End=Of=File==========================================================