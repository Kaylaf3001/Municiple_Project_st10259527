using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services.DataStructures;

namespace Municiple_Project_st10259527.Services
{
    public class ServiceRequestStatusService
    {
        //====================================================================
        // Dependencies
        //====================================================================
        #region
        private readonly IServiceRequestRepository _repo;
        public ServiceRequestStatusService(IServiceRequestRepository repo) { _repo = repo; }
        #endregion
        //====================================================================

        //====================================================================
        // Infer priority and category from text (no arrays/lists)
        //====================================================================
        public (int Priority, string Category) InferPriorityAndCategory(string title, string description, string preferredCategory = null)
        {
            return RequestPriorityInferenceService.Infer(title, description, preferredCategory);
        }
        //====================================================================

        //====================================================================
        // Calling Data Structures to Build Indexes
        //====================================================================
        public async Task<(BasicTree<ServiceRequestModel> Tree, MinHeap<int, ServiceRequestModel> Heap)> BuildIndexesAsync(int userId)
        {
            // Calling the data structures from the Services/DataStructures folder
            var tree = new BasicTree<ServiceRequestModel>();
            TreeNode<ServiceRequestModel> root = null;
            var heap = new MinHeap<int, ServiceRequestModel>();

            // Build only what the user page needs: tree + heap
            await foreach (var r in _repo.GetByUserAsync(userId))
            {
                if (root == null) { tree.SetRoot(r); root = tree.Root; }
                else tree.AddChild(root, r);
                heap.Insert(r.Priority, r);
            }

            return (tree, heap);
        }
        //====================================================================

        //====================================================================
        // Building Global Indexes
        //====================================================================
        public async Task<(BasicTree<ServiceRequestModel> Tree, MinHeap<int, ServiceRequestModel> Heap, Graph<ServiceRequestModel> Graph)> BuildGlobalIndexesAsync(
            string statusFilter = null,
            string categoryFilter = null,
            int? priorityFilter = null,
            string locationFilter = null)
        {
            // Calling the data structures from the Services/DataStructures folder
            var tree = new BasicTree<ServiceRequestModel>();
            TreeNode<ServiceRequestModel> root = null;
            var heap = new MinHeap<int, ServiceRequestModel>();
            var graph = new Graph<ServiceRequestModel>();
            Graph<ServiceRequestModel>.GraphNode prev = null;

            // await for each service request
            await foreach (var r in _repo.GetAllAsync())
            {
                // Apply filters
                if (!string.IsNullOrWhiteSpace(statusFilter))
                {
                    var sf = statusFilter.Trim();
                    // Compare by enum name
                    if (!string.Equals(r.Status.ToString(), sf, StringComparison.OrdinalIgnoreCase)) continue;
                }
                if (!string.IsNullOrWhiteSpace(categoryFilter))
                {
                    var cf = categoryFilter.Trim();
                    if (!string.Equals(r.Category ?? string.Empty, cf, StringComparison.OrdinalIgnoreCase)) continue;
                }
                if (priorityFilter.HasValue)
                {
                    if (r.Priority != priorityFilter.Value) continue;
                }
                if (!string.IsNullOrWhiteSpace(locationFilter))
                {
                    var lf = locationFilter.Trim();
                    var rl = r.Location ?? string.Empty;
                    if (rl.IndexOf(lf, StringComparison.OrdinalIgnoreCase) < 0) continue;
                }
                // Here, if the root is null, set the root to the first request
                if (root == null) { tree.SetRoot(r); root = tree.Root; }

                // else, add the request as a child of the root
                else tree.AddChild(root, r);

                // Insert into Heap
                heap.Insert(r.Priority, r);

                // Build graph
                var node = graph.AddNode(r);
                if (prev != null) graph.AddUndirectedEdge(prev, node, 1);
                foreach (var existing in graph.Nodes())
                {
                    if (ReferenceEquals(existing, node)) continue;

                    // Base weight on priority proximity (lower = closer)
                    var w = Math.Abs((existing.Val?.Priority ?? 3) - (r?.Priority ?? 3)) + 1;

                    // Same status connection
                    if (existing.Val.Status == r.Status)
                    {
                        graph.AddUndirectedEdge(existing, node, w);
                    }

                    // Same category connection (tighter cluster)
                    var ec = existing.Val?.Category ?? string.Empty; var rc = r?.Category ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(ec) && !string.IsNullOrWhiteSpace(rc) && ec.Equals(rc, StringComparison.OrdinalIgnoreCase))
                    {
                        graph.AddUndirectedEdge(existing, node, Math.Max(1, w - 1));
                    }

                    // Same location connection (very tight)
                    var el = existing.Val?.Location ?? string.Empty; var rl = r?.Location ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(el) && !string.IsNullOrWhiteSpace(rl) && el.Equals(rl, StringComparison.OrdinalIgnoreCase))
                    {
                        graph.AddUndirectedEdge(existing, node, 1);
                    }

                    // Simple dependency hint: wiring before repaint
                    var et = (existing.Val?.Title + " " + existing.Val?.Description)?.ToLowerInvariant() ?? string.Empty;
                    var rt = (r?.Title + " " + r?.Description)?.ToLowerInvariant() ?? string.Empty;
                    bool wiringE = et.Contains("wiring") || et.Contains("electrical");
                    bool repaintE = et.Contains("paint") || et.Contains("repaint");
                    bool wiringR = rt.Contains("wiring") || rt.Contains("electrical");
                    bool repaintR = rt.Contains("paint") || rt.Contains("repaint");
                    if ((wiringE && repaintR) || (wiringR && repaintE))
                    {
                        graph.AddUndirectedEdge(existing, node, 1);
                    }
                }
                prev = node;
            }
            return (tree, heap, graph);
        }
        //====================================================================

        //====================================================================
        // Track by code using AVL tree for O(log n) lookup without arrays/lists
        //====================================================================
        public async Task<ServiceRequestModel> TrackByCodeUsingAvlAsync(string trackingCode)
        {
            if (string.IsNullOrWhiteSpace(trackingCode)) return null;

            // Build an AVL index keyed by TrackingCode
            var avl = new AvlTree<string, ServiceRequestModel>();
            await foreach (var r in _repo.GetAllAsync())
            {
                avl.Insert(r.TrackingCode, r);
            }

            // Use tree-based find (O(log n))
            return avl.Find(trackingCode);
        }
        //====================================================================

        

        //====================================================================
        // Compute per-request queue metrics: how many requests ahead in same status
        //====================================================================
        public async Task<RedBlackTree<int, (int Ahead, int Total)>> BuildQueueAheadIndexAsync(int userId)
        {
            var index = new RedBlackTree<int, (int, int)>();

            // For each of the user's requests compute metrics without materializing lists
            await foreach (var myReq in _repo.GetByUserAsync(userId))
            {
                int total = 0; int ahead = 0;

                await foreach (var r in _repo.GetAllAsync())
                {
                    if (r.Status != myReq.Status) continue;
                    total++;
                    if (r.Priority < myReq.Priority) ahead++;
                }

                index.Insert(myReq.RequestId, (ahead, total));
            }

            return index;
        }
        //====================================================================
    }
}
//==========================End=Of=File========================================
