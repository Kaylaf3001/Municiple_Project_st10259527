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
        // Calling Data Structures to Build Indexes
        //====================================================================
        public async Task<(BasicTree<ServiceRequestModel> Tree, AvlTree<string, ServiceRequestModel> Avl, RedBlackTree<int, ServiceRequestModel> Rb, MinHeap<int, ServiceRequestModel> Heap, Graph<ServiceRequestModel> Graph)> BuildIndexesAsync(int userId)
        {

            // Calling the data structures from the Services/DataStructures folder
            var tree = new BasicTree<ServiceRequestModel>();
            TreeNode<ServiceRequestModel> root = null;
            var avl = new AvlTree<string, ServiceRequestModel>();
            var rb = new RedBlackTree<int, ServiceRequestModel>();
            var heap = new MinHeap<int, ServiceRequestModel>();
            var graph = new Graph<ServiceRequestModel>();
            Graph<ServiceRequestModel>.GraphNode prev = null;

            // await for each service request for the user
            await foreach (var r in _repo.GetByUserAsync(userId))
            {

                // Here, if the root is null, set the root to the first request
                if (root == null) { tree.SetRoot(r); root = tree.Root; }

                // else, add the request as a child of the root
                else tree.AddChild(root, r);

                // Insert into AVL, RB, Heap, and Graph
                avl.Insert(r.TrackingCode, r);
                rb.Insert(r.RequestId, r);
                heap.Insert(r.Priority, r);

                // For the graph, add an undirected edge between the previous and current node
                var node = graph.AddNode(r);

                // If prev is not null, add an undirected edge
                if (prev != null) graph.AddUndirectedEdge(prev, node, 1);
                prev = node;
            }

            return (tree, avl, rb, heap, graph);
        }
        //====================================================================

        //====================================================================
        // Building Global Indexes
        //====================================================================
        public async Task<(BasicTree<ServiceRequestModel> Tree, MinHeap<int, ServiceRequestModel> Heap)> BuildGlobalIndexesAsync()
        {
            // Calling the data structures from the Services/DataStructures folder
            var tree = new BasicTree<ServiceRequestModel>();
            TreeNode<ServiceRequestModel> root = null;
            var heap = new MinHeap<int, ServiceRequestModel>();

            // await for each service request
            await foreach (var r in _repo.GetAllAsync())
            {
                // Here, if the root is null, set the root to the first request
                if (root == null) { tree.SetRoot(r); root = tree.Root; }

                // else, add the request as a child of the root
                else tree.AddChild(root, r);

                // Insert into Heap
                heap.Insert(r.Priority, r);
            }
            return (tree, heap);
        }
        //====================================================================

        //====================================================================
        // Track Service Request by Tracking Code
        //====================================================================
        public async Task<ServiceRequestModel> TrackByCodeAsync(string trackingCode)
        {
            return await _repo.GetByTrackingCodeAsync(trackingCode);
        }
        //====================================================================
    }
}
//==========================End=Of=File========================================
