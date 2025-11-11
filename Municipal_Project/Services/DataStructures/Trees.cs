using System;
using System.Collections;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Services.DataStructures
{
    //===============================================================================
    // Trees
    //===============================================================================
    public class TreeNode<T>
    {
        public T Value { get; set; }
        public TreeNode<T> FirstChild { get; set; }
        public TreeNode<T> NextSibling { get; set; }
        public TreeNode(T value) { Value = value; }
    }
    //===============================================================================

    //===============================================================================
    // A basic tree structure with first child/next sibling representation
    // I implements IEnumerable<T> for easy traversal
    // This method is used to manage hierarchical relationships between service requests
    //===============================================================================
    public class BasicTree<T> : IEnumerable<T>
    {
        // setting root
        public TreeNode<T> Root { get; private set; }

        // add child to a parent node
        public void SetRoot(T value) { Root = new TreeNode<T>(value); }

        // add child to a parent node
        public void AddChild(TreeNode<T> parent, T value)
        {

            // if no child, set as first child
            if (parent.FirstChild == null) parent.FirstChild = new TreeNode<T>(value);

            // else, traverse to last sibling and add there
            else
            {
                // traverse to last sibling
                var n = parent.FirstChild;

                // add new sibling
                while (n.NextSibling != null) n = n.NextSibling;

                // add new sibling
                n.NextSibling = new TreeNode<T>(value);
            }
        }
        //============================================================================

        // IEnumerable implementation for tree traversal
        public IEnumerator<T> GetEnumerator() { return Traverse(Root).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        // pre-order traversal
        IEnumerable<T> Traverse(TreeNode<T> node)
        {
            if (node == null) yield break;
            yield return node.Value;
            var child = node.FirstChild;
            while (child != null)
            {
                foreach (var v in Traverse(child)) yield return v;
                child = child.NextSibling;
            }
        }
    }
    //===============================================================================


    //===============================================================================
    // Binary Search Tree, AVL Tree, Red-Black Tree, Min-Heap, Graph
    //===============================================================================
    public class BinaryNode<T>
    {
        public T Value;
        public BinaryNode<T> Left;
        public BinaryNode<T> Right;
        public int Height;
        public bool Red;
        public BinaryNode(T v) { Value = v; Height = 1; }
    }
    //===============================================================================

    //===============================================================================
    // Ikey is used to comapare keys in the tree
    //===============================================================================
    public interface IKey<TK> { int Compare(TK a, TK b); }
    //===============================================================================

    //===============================================================================
    // ComparableKey uses IComparable to compare keys
    //===============================================================================

    public class ComparableKey<TK> : IKey<TK> where TK : IComparable<TK>
    {
        // compare two keys
        public int Compare(TK a, TK b) => a.CompareTo(b);
    }
    //===============================================================================

    //===============================================================================
    // Binary Search Tree
    //===============================================================================
    public class BinarySearchTree<TK, TV> where TK : IComparable<TK>
    {
        // Creating a new ComparableKey instance to compare keys
        protected readonly IKey<TK> K = new ComparableKey<TK>();

        // Root node of the tree
        protected BinaryNode<(TK Key, TV Val)> Root;

        public virtual void Insert(TK key, TV val) { Root = Insert(Root, key, val); }

        public virtual TV Find(TK key)
        {
            // Start from the root node
            var n = Root;

            // Traverse the tree
            while (n != null)
            {
                // Compare the key with the current node's key
                var c = K.Compare(key, n.Value.Key);
                if (c == 0) return n.Value.Val;
                n = c < 0 ? n.Left : n.Right;
            }
            return default;
        }
        // Recursive insert helper
        // Inserts a new node or updates an existing node with the given key and value
        protected virtual BinaryNode<(TK, TV)> Insert(BinaryNode<(TK, TV)> n, TK k, TV v)
        {
            if (n == null) return new BinaryNode<(TK, TV)>((k, v));
            var c = K.Compare(k, n.Value.Item1);
            if (c < 0) n.Left = Insert(n.Left, k, v);
            else if (c > 0) n.Right = Insert(n.Right, k, v);
            else n.Value = (k, v);
            return n;
        }
    }
    //===============================================================================

    //===============================================================================
    // AVL Tree
    //===============================================================================

    public class AvlTree<TK, TV> : BinarySearchTree<TK, TV> where TK : IComparable<TK>
    {
        // Height of a node
        int H(BinaryNode<(TK, TV)> n) => n?.Height ?? 0;

        // Balance factor of a node
        int B(BinaryNode<(TK, TV)> n) => n == null ? 0 : H(n.Left) - H(n.Right);

        // Update height of a node
        void U(BinaryNode<(TK, TV)> n) { if (n != null) n.Height = Math.Max(H(n.Left), H(n.Right)) + 1; }

        // Right-Left rotation
        BinaryNode<(TK, TV)> RL(BinaryNode<(TK, TV)> y)
        { var x = y.Left; var T2 = x.Right; x.Right = y; y.Left = T2; U(y); U(x); return x; }

        // Left-Right rotation
        BinaryNode<(TK, TV)> RR(BinaryNode<(TK, TV)> x)
        { var y = x.Right; var T2 = y.Left; y.Left = x; x.Right = T2; U(x); U(y); return y; }

        // Insert with balancing
        protected override BinaryNode<(TK, TV)> Insert(BinaryNode<(TK, TV)> n, TK k, TV v)
        {
            n = base.Insert(n, k, v);
            U(n);
            var bal = B(n);
            if (bal > 1 && K.Compare(k, n.Left.Value.Item1) < 0) return RL(n);
            if (bal < -1 && K.Compare(k, n.Right.Value.Item1) > 0) return RR(n);
            if (bal > 1 && K.Compare(k, n.Left.Value.Item1) > 0) { n.Left = RR(n.Left); return RL(n); }
            if (bal < -1 && K.Compare(k, n.Right.Value.Item1) < 0) { n.Right = RL(n.Right); return RR(n); }
            return n;
        }
    }
    //===============================================================================

    //===============================================================================
    // Red-Black Tree
    //===============================================================================

    public class RedBlackTree<TK, TV> : BinarySearchTree<TK, TV> where TK : IComparable<TK>
    {
        // Check if a node is red
        bool IsRed(BinaryNode<(TK, TV)> n) => n != null && n.Red;

        // Left rotation
        BinaryNode<(TK, TV)> RotateLeft(BinaryNode<(TK, TV)> h)
        { var x = h.Right; h.Right = x.Left; x.Left = h; x.Red = h.Red; h.Red = true; return x; }

        // Right rotation
        BinaryNode<(TK, TV)> RotateRight(BinaryNode<(TK, TV)> h)
        { var x = h.Left; h.Left = x.Right; x.Right = h; x.Red = h.Red; h.Red = true; return x; }

        // Flip colors
        void FlipColors(BinaryNode<(TK, TV)> h)
        { h.Red = !h.Red; if (h.Left != null) h.Left.Red = !h.Left.Red; if (h.Right != null) h.Right.Red = !h.Right.Red; }

        // Insert with balancing
        protected override BinaryNode<(TK, TV)> Insert(BinaryNode<(TK, TV)> h, TK k, TV v)
        {
            if (h == null) { var n = new BinaryNode<(TK, TV)>((k, v)); n.Red = true; return n; }
            var c = K.Compare(k, h.Value.Item1);
            if (c < 0) h.Left = Insert(h.Left, k, v);
            else if (c > 0) h.Right = Insert(h.Right, k, v);
            else h.Value = (k, v);
            if (IsRed(h.Right) && !IsRed(h.Left)) h = RotateLeft(h);
            if (IsRed(h.Left) && IsRed(h.Left.Left)) h = RotateRight(h);
            if (IsRed(h.Left) && IsRed(h.Right)) FlipColors(h);
            return h;
        }

        // Override Insert to ensure root is always black
        public override void Insert(TK key, TV val)
        {
            Root = Insert(Root, key, val);
            if (Root != null) Root.Red = false;
        }
    }
    //===============================================================================


    //===============================================================================
    // Min-Heap
    //===============================================================================

    public class MinHeap<TK, TV> where TK : IComparable<TK>
    {
        // Heap Node
        class HNode { public TK Key; public TV Val; public HNode Parent; public HNode Left; public HNode Right; }

        // Root node and count of nodes
        HNode root; int count;

        // Number of elements in the heap
        public int Count => count;

        // Insert key-value pair into the heap
        public void Insert(TK k, TV v)
        {
            if (root == null) { root = new HNode { Key = k, Val = v }; count = 1; return; }
            var path = count + 1; var stack = new Stack<bool>();
            while (path > 1) { stack.Push((path & 1) == 1); path >>= 1; }
            var cur = root;
            while (stack.Count > 1) { cur = stack.Pop() ? cur.Right : cur.Left; }
            var node = new HNode { Key = k, Val = v, Parent = cur };
            if (stack.Pop()) cur.Right = node; else cur.Left = node;
            count++;
            BubbleUp(node);
        }

        // Bubble up the node to maintain heap property
        void BubbleUp(HNode n)
        {
            while (n.Parent != null && n.Key.CompareTo(n.Parent.Key) < 0)
            { Swap(n, n.Parent); n = n.Parent; }
        }

        // Bubble down the node to maintain heap property
        void Swap(HNode a, HNode b)
        { var tk = a.Key; var tv = a.Val; a.Key = b.Key; a.Val = b.Val; b.Key = tk; b.Val = tv; }

        // Remove and return the minimum key-value pair from the heap
        public (TK, TV) Peek() { if (root == null) return (default, default); return (root.Key, root.Val); }

        // Remove and return the minimum key-value pair from the heap
        public IEnumerable<TV> InOrder()
        {
            foreach (var v in InOrder(root)) yield return v;
        }

        // In-order traversal helper
        IEnumerable<TV> InOrder(HNode n)
        {
            if (n == null) yield break; foreach (var v in InOrder(n.Left)) yield return v; yield return n.Val; foreach (var v in InOrder(n.Right)) yield return v;
        }
    }
    //===============================================================================


    //===============================================================================
    // Graph
    //===============================================================================
    /// <summary>
    /// Adjacency list graph with lightweight node and edge structures.
    /// </summary>
    /// <remarks>
    /// - AddNode O(1)
    /// - AddUndirectedEdge O(1)
    /// - DFS/BFS O(V+E)
    /// - PrimMst O(V * E) in this simple implementation
    /// </remarks>
    public class Graph<T>
    {
        // Edge class representing a connection between nodes
        public class Edge { public GraphNode To; public int W; public Edge Next; }

        // Graph node class
        public class GraphNode
        {
            public T Val; public Edge First; public GraphNode Next;
            public GraphNode(T v) { Val = v; }
        }

        // Head of the graph node list
        GraphNode head;

        // Add a new node to the graph
        public GraphNode AddNode(T v)
        {
            var n = new GraphNode(v) { Next = head }; head = n; return n;
        }

        // Add an undirected edge between two nodes with a given weight
        public void AddUndirectedEdge(GraphNode a, GraphNode b, int w)
        {
            a.First = new Edge { To = b, W = w, Next = a.First };
            b.First = new Edge { To = a, W = w, Next = b.First };
        }

        // Depth-First Search (DFS) traversal of the graph
        public IEnumerable<T> Dfs(GraphNode start)
        {
            var visited = new HashSet<GraphNode>(); var stack = new Stack<GraphNode>(); stack.Push(start);
            while (stack.Count > 0)
            {
                var n = stack.Pop(); if (!visited.Add(n)) continue; yield return n.Val;
                var e = n.First; while (e != null) { stack.Push(e.To); e = e.Next; }
            }
        }

        // Breadth-First Search (BFS) traversal of the graph
        public IEnumerable<T> Bfs(GraphNode start)
        {
            var visited = new HashSet<GraphNode>(); var q = new Queue<GraphNode>(); q.Enqueue(start);
            while (q.Count > 0)
            {
                var n = q.Dequeue(); if (!visited.Add(n)) continue; yield return n.Val;
                var e = n.First; while (e != null) { q.Enqueue(e.To); e = e.Next; }
            }
        }

        // Enumerate all nodes for visualization
        public IEnumerable<GraphNode> Nodes()
        {
            var n = head; while (n != null) { yield return n; n = n.Next; }
        }

        // Enumerate all edges (directed listing of undirected edges)
        public IEnumerable<(GraphNode From, GraphNode To, int W)> Edges()
        {
            var n = head; while (n != null) { var e = n.First; while (e != null) { yield return (n, e.To, e.W); e = e.Next; } n = n.Next; }
        }

        // Prim's Minimum Spanning Tree (MST) algorithm
        public IEnumerable<(GraphNode, GraphNode, int)> PrimMst()
        {
            // initialize
            var visited = new HashSet<GraphNode>();
            var p = head; if (p == null) yield break; visited.Add(p);

            // In this loop we repeatedly find the minimum weight edge that connects a visited node to an unvisited node
            while (true)
            {

                // find the best edge
                GraphNode bestU = null, bestV = null; int bestW = int.MaxValue; bool found = false;
                var u = head;

                // iterate through all nodes
                while (u != null)
                {

                    // if the node is visited
                    if (visited.Contains(u))
                    {
                        var e = u.First;

                        // iterate through all edges of the node
                        while (e != null)
                        {
                            if (!visited.Contains(e.To) && e.W < bestW) { bestW = e.W; bestU = u; bestV = e.To; found = true; }
                            e = e.Next;
                        }
                    }
                    // move to the next node
                    u = u.Next;
                }
                // if no edge found, break
                if (!found) yield break;
                visited.Add(bestV);
                yield return (bestU, bestV, bestW);
            }
        }
        //===============================================================================
    }
}
//===============================End=Of=File==============================================
