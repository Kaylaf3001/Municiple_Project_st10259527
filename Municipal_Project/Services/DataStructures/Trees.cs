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
                var currentSibling = parent.FirstChild;

                // add new sibling
                while (currentSibling.NextSibling != null) currentSibling = currentSibling.NextSibling;

                // add new sibling
                currentSibling.NextSibling = new TreeNode<T>(value);
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
            var childNode = node.FirstChild;
            while (childNode != null)
            {
                foreach (var value in Traverse(childNode)) yield return value;
                childNode = childNode.NextSibling;
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
    // Plain Binary Tree wrapper (level-order insert and traversals)
    //===============================================================================
    public class BinaryTree<T> : IEnumerable<T>
    {
        public BinaryNode<T> Root { get; private set; }

        // Insert by level-order to keep tree as complete as possible
        public void InsertLevelOrder(T value)
        {
            var newNode = new BinaryNode<T>(value);
            if (Root == null) { Root = newNode; return; }

            var q = new Queue<BinaryNode<T>>();
            q.Enqueue(Root);
            while (q.Count > 0)
            {
                var current = q.Dequeue();
                if (current.Left == null) { current.Left = newNode; return; }
                if (current.Right == null) { current.Right = newNode; return; }
                q.Enqueue(current.Left);
                q.Enqueue(current.Right);
            }
        }

        //============================================================================
        // Traversals
        //============================================================================
        public IEnumerable<T> InOrder()
        {
            foreach (var v in InOrder(Root)) yield return v;
        }
        // ============================================================================

        //============================================================================
        IEnumerable<T> InOrder(BinaryNode<T> node)
        {
            if (node == null) yield break;
            foreach (var v in InOrder(node.Left)) yield return v;
            yield return node.Value;
            foreach (var v in InOrder(node.Right)) yield return v;
        }
        //============================================================================

        //============================================================================
        public IEnumerable<T> PreOrder()
        {
            foreach (var v in PreOrder(Root)) yield return v;
        }
        //============================================================================

        //============================================================================
        IEnumerable<T> PreOrder(BinaryNode<T> node)
        {
            if (node == null) yield break;
            yield return node.Value;
            foreach (var v in PreOrder(node.Left)) yield return v;
            foreach (var v in PreOrder(node.Right)) yield return v;
        }
        //============================================================================

        //============================================================================
        public IEnumerable<T> PostOrder()
        {
            foreach (var v in PostOrder(Root)) yield return v;
        }
        //============================================================================

        //============================================================================
        IEnumerable<T> PostOrder(BinaryNode<T> node)
        {
            if (node == null) yield break;
            foreach (var v in PostOrder(node.Left)) yield return v;
            foreach (var v in PostOrder(node.Right)) yield return v;
            yield return node.Value;
        }
        //============================================================================

        //============================================================================
        public IEnumerable<T> LevelOrder()
        {
            if (Root == null) yield break;
            var q = new Queue<BinaryNode<T>>();
            q.Enqueue(Root);
            while (q.Count > 0)
            {
                var current = q.Dequeue();
                yield return current.Value;
                if (current.Left != null) q.Enqueue(current.Left);
                if (current.Right != null) q.Enqueue(current.Right);
            }
        }
        //============================================================================

        public IEnumerator<T> GetEnumerator() { return LevelOrder().GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
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
            var current = Root;

            // Traverse the tree
            while (current != null)
            {
                // Compare the key with the current node's key
                var cmp = K.Compare(key, current.Value.Key);
                if (cmp == 0) return current.Value.Val;
                current = cmp < 0 ? current.Left : current.Right;
            }
            return default;
        }
        // Recursive insert helper
        // Inserts a new node or updates an existing node with the given key and value
        protected virtual BinaryNode<(TK, TV)> Insert(BinaryNode<(TK, TV)> node, TK key, TV value)
        {
            if (node == null) return new BinaryNode<(TK, TV)>((key, value));
            var cmp = K.Compare(key, node.Value.Item1);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);
            else if (cmp > 0) node.Right = Insert(node.Right, key, value);
            else node.Value = (key, value);
            return node;
        }
    }
    //===============================================================================

    //===============================================================================
    // AVL Tree
    //===============================================================================
    public class AvlTree<TK, TV> : BinarySearchTree<TK, TV> where TK : IComparable<TK>
    {
        // Height of a node
        int HeightOf(BinaryNode<(TK, TV)> node) => node?.Height ?? 0;

        // Balance factor of a node
        int BalanceFactor(BinaryNode<(TK, TV)> node) => node == null ? 0 : HeightOf(node.Left) - HeightOf(node.Right);

        // Update height of a node
        void UpdateHeight(BinaryNode<(TK, TV)> node) { if (node != null) node.Height = Math.Max(HeightOf(node.Left), HeightOf(node.Right)) + 1; }

        // Right-Left rotation
        BinaryNode<(TK, TV)> RotateRight(BinaryNode<(TK, TV)> y)
        { var x = y.Left; var T2 = x.Right; x.Right = y; y.Left = T2; UpdateHeight(y); UpdateHeight(x); return x; }

        // Left-Right rotation
        BinaryNode<(TK, TV)> RotateLeft(BinaryNode<(TK, TV)> x)
        { var y = x.Right; var T2 = y.Left; y.Left = x; x.Right = T2; UpdateHeight(x); UpdateHeight(y); return y; }

        // Insert with balancing
        protected override BinaryNode<(TK, TV)> Insert(BinaryNode<(TK, TV)> node, TK key, TV value)
        {
            node = base.Insert(node, key, value);
            UpdateHeight(node);
            var balance = BalanceFactor(node);
            if (balance > 1 && K.Compare(key, node.Left.Value.Item1) < 0) return RotateRight(node);
            if (balance < -1 && K.Compare(key, node.Right.Value.Item1) > 0) return RotateLeft(node);
            if (balance > 1 && K.Compare(key, node.Left.Value.Item1) > 0) { node.Left = RotateLeft(node.Left); return RotateRight(node); }
            if (balance < -1 && K.Compare(key, node.Right.Value.Item1) < 0) { node.Right = RotateRight(node.Right); return RotateLeft(node); }
            return node;
        }
    }
    //===============================================================================

    //===============================================================================
    // Red-Black Tree
    //===============================================================================
    public class RedBlackTree<TK, TV> : BinarySearchTree<TK, TV> where TK : IComparable<TK>
    {
        // Check if a node is red
        bool IsRed(BinaryNode<(TK, TV)> node) => node != null && node.Red;

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
        protected override BinaryNode<(TK, TV)> Insert(BinaryNode<(TK, TV)> node, TK key, TV value)
        {
            if (node == null) { var created = new BinaryNode<(TK, TV)>((key, value)); created.Red = true; return created; }
            var cmp = K.Compare(key, node.Value.Item1);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);
            else if (cmp > 0) node.Right = Insert(node.Right, key, value);
            else node.Value = (key, value);
            if (IsRed(node.Right) && !IsRed(node.Left)) node = RotateLeft(node);
            if (IsRed(node.Left) && IsRed(node.Left.Left)) node = RotateRight(node);
            if (IsRed(node.Left) && IsRed(node.Right)) FlipColors(node);
            return node;
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

        // Bubble up the node to maintain heap property
        void BubbleUp(HNode node)
        {
            while (node.Parent != null && node.Key.CompareTo(node.Parent.Key) < 0)
            { Swap(node, node.Parent); node = node.Parent; }
        }

        // Bubble down the node to maintain heap property
        void Swap(HNode a, HNode b)
        { var tk = a.Key; var tv = a.Val; a.Key = b.Key; a.Val = b.Val; b.Key = tk; b.Val = tv; }

        // Remove and return the minimum key-value pair from the heap
        public (TK, TV) Peek() { if (root == null) return (default, default); return (root.Key, root.Val); }

        // Remove and return the minimum key-value pair from the heap
        public IEnumerable<TV> InOrder()
        {
            foreach (var value in InOrder(root)) yield return value;
        }

        // In-order traversal helper
        IEnumerable<TV> InOrder(HNode node)
        {
            if (node == null) yield break; foreach (var leftVal in InOrder(node.Left)) yield return leftVal; yield return node.Val; foreach (var rightVal in InOrder(node.Right)) yield return rightVal;
        }

        // Extract the minimum element (root) and re-heapify
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
    }
    //===============================================================================


    //===============================================================================
    // Graph
    //===============================================================================
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
        public GraphNode AddNode(T value)
        {
            var node = new GraphNode(value) { Next = head }; head = node; return node;
        }

        // Add an undirected edge between two nodes with a given weight
        public void AddUndirectedEdge(GraphNode nodeA, GraphNode nodeB, int weight)
        {
            nodeA.First = new Edge { To = nodeB, W = weight, Next = nodeA.First };
            nodeB.First = new Edge { To = nodeA, W = weight, Next = nodeB.First };
        }

        // Depth-First Search (DFS) traversal of the graph
        public IEnumerable<T> Dfs(GraphNode start)
        {
            var visited = new HashSet<GraphNode>(); var stack = new Stack<GraphNode>(); stack.Push(start);
            while (stack.Count > 0)
            {
                var current = stack.Pop(); if (!visited.Add(current)) continue; yield return current.Val;
                var edge = current.First; while (edge != null) { stack.Push(edge.To); edge = edge.Next; }
            }
        }

        // Breadth-First Search (BFS) traversal of the graph
        public IEnumerable<T> Bfs(GraphNode start)
        {
            var visited = new HashSet<GraphNode>(); var q = new Queue<GraphNode>(); q.Enqueue(start);
            while (q.Count > 0)
            {
                var current = q.Dequeue(); if (!visited.Add(current)) continue; yield return current.Val;
                var edge = current.First; while (edge != null) { q.Enqueue(edge.To); edge = edge.Next; }
            }
        }

        // Return neighbors and weights for a node
        public IEnumerable<(GraphNode To, int W)> Neighbors(GraphNode node)
        {
            var edge = node?.First; while (edge != null) { yield return (edge.To, edge.W); edge = edge.Next; }
        }

        // Add or update an undirected edge, keeping the minimal weight and avoiding duplicates
        public void AddOrUpdateUndirectedEdge(GraphNode fromNode, GraphNode toNode, int weight)
        {
            if (fromNode == null || toNode == null || ReferenceEquals(fromNode, toNode)) return;

            void Upsert(GraphNode from, GraphNode to)
            {
                // Try find existing edge
                var edge = from.First; Edge prev = null; while (edge != null)
                {
                    if (ReferenceEquals(edge.To, to))
                    {
                        // Keep minimal weight
                        if (weight < edge.W) edge.W = weight;
                        return;
                    }
                    prev = edge; edge = edge.Next;
                }
                // Not found: prepend new edge
                from.First = new Edge { To = to, W = weight, Next = from.First };
            }

            Upsert(fromNode, toNode);
            Upsert(toNode, fromNode);
        }

        // Enumerate all nodes for visualization
        public IEnumerable<GraphNode> Nodes()
        {
            var node = head; while (node != null) { yield return node; node = node.Next; }
        }

        // Enumerate all edges (directed listing of undirected edges)
        public IEnumerable<(GraphNode From, GraphNode To, int W)> Edges()
        {
            var node = head; while (node != null) { var edge = node.First; while (edge != null) { yield return (node, edge.To, edge.W); edge = edge.Next; } node = node.Next; }
        }

        // Prim's Minimum Spanning Tree (MST) algorithm
        public IEnumerable<(GraphNode, GraphNode, int)> PrimMst()
        {
            // initialize
            var visited = new HashSet<GraphNode>();
            var firstNode = head; if (firstNode == null) yield break; visited.Add(firstNode);

            // In this loop we repeatedly find the minimum weight edge that connects a visited node to an unvisited node
            while (true)
            {

                // find the best edge
                GraphNode bestFrom = null, bestTo = null; int bestWeight = int.MaxValue; bool found = false;
                var currentNode = head;

                // iterate through all nodes
                while (currentNode != null)
                {

                    // if the node is visited
                    if (visited.Contains(currentNode))
                    {
                        var edge = currentNode.First;

                        // iterate through all edges of the node
                        while (edge != null)
                        {
                            if (!visited.Contains(edge.To) && edge.W < bestWeight) { bestWeight = edge.W; bestFrom = currentNode; bestTo = edge.To; found = true; }
                            edge = edge.Next;
                        }
                    }
                    // move to the next node
                    currentNode = currentNode.Next;
                }
                // if no edge found, break
                if (!found) yield break;
                visited.Add(bestTo);
                yield return (bestFrom, bestTo, bestWeight);
            }
        }

        // Prim's MST starting from a provided node
        public IEnumerable<(GraphNode, GraphNode, int)> PrimMst(GraphNode start)
        {
            var visited = new HashSet<GraphNode>();
            if (start == null) yield break;
            visited.Add(start);

            while (true)
            {
                GraphNode bestFrom = null, bestTo = null; int bestWeight = int.MaxValue; bool found = false;

                var currentNode = head;
                while (currentNode != null)
                {
                    if (visited.Contains(currentNode))
                    {
                        var edge = currentNode.First;
                        while (edge != null)
                        {
                            if (!visited.Contains(edge.To) && edge.W < bestWeight) { bestWeight = edge.W; bestFrom = currentNode; bestTo = edge.To; found = true; }
                            edge = edge.Next;
                        }
                    }
                    currentNode = currentNode.Next;
                }

                if (!found) yield break;
                visited.Add(bestTo);
                yield return (bestFrom, bestTo, bestWeight);
            }
        }
        
        //===============================================================================
    }
}
//===============================End=Of=File==============================================
