using System;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Services.DataStructures
{
    //=============================================================================
    // Min-Heap Implementation
    //=============================================================================
    public class MinHeap<TK, TV> where TK : IComparable<TK>
    {
        //=============================================================================
        // Heap Node Definition
        //=============================================================================
        #region
        class HNode { public TK Key; public TV Val; public HNode Parent; public HNode Left; public HNode Right; }

        HNode root; int count;
        public int Count => count;
        #endregion
        //=============================================================================

        //=============================================================================
        // Insert Key-Value Pair into Min-Heap
        //=============================================================================
        public void Insert(TK key, TV value)
        {
            if (root == null) { root = new HNode { Key = key, Val = value }; count = 1; return; }
            var pathIndex = count + 1; var stack = new Stack<bool>();
            while (pathIndex > 1) { stack.Push((pathIndex & 1) == 1); pathIndex >>= 1; }
            var current = root;
            while (stack.Count > 1) { current = stack.Pop() ? current.Right : current.Left; }
            var node = new HNode { Key = key, Val = value, Parent = current };
            if (stack.Pop()) current.Right = node; else current.Left = node;
            count++;
            BubbleUp(node);
        }
        //=============================================================================

        //=============================================================================
        // Bubble Up to Maintain Heap Property
        //=============================================================================
        void BubbleUp(HNode node)
        {
            while (node.Parent != null && node.Key.CompareTo(node.Parent.Key) < 0)
            { Swap(node, node.Parent); node = node.Parent; }
        }
        //=============================================================================

        //=============================================================================
        // Swap Keys and Values of Two Nodes
        //=============================================================================
        void Swap(HNode a, HNode b)
        { var tk = a.Key; var tv = a.Val; a.Key = b.Key; a.Val = b.Val; b.Key = tk; b.Val = tv; }
        //=============================================================================

        //=============================================================================
        // Peek at Minimum Key-Value Pair without Removal
        //=============================================================================
        public (TK, TV) Peek() { if (root == null) return (default, default); return (root.Key, root.Val); }
        //=============================================================================

        //=============================================================================
        // In-Order Traversal of Heap
        //=============================================================================
        public IEnumerable<TV> InOrder()
        {
            foreach (var value in InOrder(root)) yield return value;
        }
        //=============================================================================

        //=============================================================================
        // Recursive In-Order Traversal Helper
        //=============================================================================
        IEnumerable<TV> InOrder(HNode node)
        {
            if (node == null) yield break; foreach (var leftVal in InOrder(node.Left)) yield return leftVal; yield return node.Val; foreach (var rightVal in InOrder(node.Right)) yield return rightVal;
        }
        //=============================================================================

        //=============================================================================
        // Extract Minimum Key-Value Pair from Heap
        //=============================================================================
        public (TK, TV) ExtractMin()
        {
            if (root == null) return (default, default);
            var min = (root.Key, root.Val);
            if (count == 1) { root = null; count = 0; return min; }

            var pathIndex = count; var stack = new Stack<bool>();
            while (pathIndex > 1) { stack.Push((pathIndex & 1) == 1); pathIndex >>= 1; }
            var current = root; HNode parent = null;
            while (stack.Count > 0)
            {
                parent = current;
                current = stack.Pop() ? current.Right : current.Left;
            }

            root.Key = current.Key; root.Val = current.Val;
            if (parent.Right == current) parent.Right = null; else parent.Left = null;
            count--;
            BubbleDown(root);
            return min;
        }
        //=============================================================================

        //=============================================================================
        // Bubble Down to Maintain Heap Property
        //=============================================================================
        void BubbleDown(HNode node)
        {
            while (node != null)
            {
                HNode smallest = node;
                if (node.Left != null && node.Left.Key.CompareTo(smallest.Key) < 0) smallest = node.Left;
                if (node.Right != null && node.Right.Key.CompareTo(smallest.Key) < 0) smallest = node.Right;
                if (ReferenceEquals(smallest, node)) break;
                Swap(node, smallest);
                node = smallest;
            }
        }
        //=============================================================================

        //=============================================================================
        // Get Top K Minimum Values from Heap
        //=============================================================================
        public IEnumerable<TV> TopK(int k)
        {
            if (k <= 0 || root == null) yield break;

            var temp = new MinHeap<TK, TV>();
            var q = new Queue<HNode>();
            q.Enqueue(root);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                temp.Insert(n.Key, n.Val);
                if (n.Left != null) q.Enqueue(n.Left);
                if (n.Right != null) q.Enqueue(n.Right);
            }

            int emitted = 0;
            while (emitted < k && temp.Count > 0)
            {
                var pair = temp.ExtractMin();
                yield return pair.Item2;
                emitted++;
            }
        }
        //=============================================================================
    }
}
//==================================End=Of=File=========================================
