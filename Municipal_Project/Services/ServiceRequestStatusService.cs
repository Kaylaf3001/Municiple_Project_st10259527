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
        private readonly IServiceRequestRepository _repo;
        public ServiceRequestStatusService(IServiceRequestRepository repo) { _repo = repo; }

        public async Task<(BasicTree<ServiceRequestModel> Tree, AvlTree<string, ServiceRequestModel> Avl, RedBlackTree<int, ServiceRequestModel> Rb, MinHeap<int, ServiceRequestModel> Heap, Graph<ServiceRequestModel> Graph)> BuildIndexesAsync(int userId)
        {
            var tree = new BasicTree<ServiceRequestModel>();
            TreeNode<ServiceRequestModel> root = null;
            var avl = new AvlTree<string, ServiceRequestModel>();
            var rb = new RedBlackTree<int, ServiceRequestModel>();
            var heap = new MinHeap<int, ServiceRequestModel>();
            var graph = new Graph<ServiceRequestModel>();
            Graph<ServiceRequestModel>.GraphNode prev = null;

            await foreach (var r in _repo.GetByUserAsync(userId))
            {
                if (root == null) { tree.SetRoot(r); root = tree.Root; }
                else tree.AddChild(root, r);

                avl.Insert(r.TrackingCode, r);
                rb.Insert(r.RequestId, r);
                heap.Insert(r.Priority, r);

                var node = graph.AddNode(r);
                if (prev != null) graph.AddUndirectedEdge(prev, node, 1);
                prev = node;
            }

            return (tree, avl, rb, heap, graph);
        }

        public async Task<(BasicTree<ServiceRequestModel> Tree, MinHeap<int, ServiceRequestModel> Heap)> BuildGlobalIndexesAsync()
        {
            var tree = new BasicTree<ServiceRequestModel>();
            TreeNode<ServiceRequestModel> root = null;
            var heap = new MinHeap<int, ServiceRequestModel>();
            await foreach (var r in _repo.GetAllAsync())
            {
                if (root == null) { tree.SetRoot(r); root = tree.Root; }
                else tree.AddChild(root, r);
                heap.Insert(r.Priority, r);
            }
            return (tree, heap);
        }

        public async Task<ServiceRequestModel> TrackByCodeAsync(string trackingCode)
        {
            return await _repo.GetByTrackingCodeAsync(trackingCode);
        }
    }
}
