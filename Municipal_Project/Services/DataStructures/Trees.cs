using System;
using System.Collections;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Services.DataStructures
{
    public class TreeNode<T>
    {
        public T Value { get; set; }
        public TreeNode<T> FirstChild { get; set; }
        public TreeNode<T> NextSibling { get; set; }
        public TreeNode(T value) { Value = value; }
    }

    public class BasicTree<T> : IEnumerable<T>
    {
        public TreeNode<T> Root { get; private set; }
        public void SetRoot(T value) { Root = new TreeNode<T>(value); }
        public void AddChild(TreeNode<T> parent, T value)
        {
            if (parent.FirstChild == null) parent.FirstChild = new TreeNode<T>(value);
            else
            {
                var n = parent.FirstChild;
                while (n.NextSibling != null) n = n.NextSibling;
                n.NextSibling = new TreeNode<T>(value);
            }
        }
        public IEnumerator<T> GetEnumerator() { return Traverse(Root).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
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

    public class BinaryNode<T>
    {
        public T Value;
        public BinaryNode<T> Left;
        public BinaryNode<T> Right;
        public int Height;
        public bool Red;
        public BinaryNode(T v) { Value = v; Height = 1; }
    }

    public interface IKey<TK> { int Compare(TK a, TK b); }

    public class ComparableKey<TK> : IKey<TK> where TK : IComparable<TK>
    {
        public int Compare(TK a, TK b) => a.CompareTo(b);
    }

    public class BinarySearchTree<TK, TV> where TK : IComparable<TK>
    {
        protected readonly IKey<TK> K = new ComparableKey<TK>();
        protected BinaryNode<(TK Key, TV Val)> Root;
        public virtual void Insert(TK key, TV val) { Root = Insert(Root, key, val); }
        public virtual TV Find(TK key)
        {
            var n = Root;
            while (n != null)
            {
                var c = K.Compare(key, n.Value.Key);
                if (c == 0) return n.Value.Val;
                n = c < 0 ? n.Left : n.Right;
            }
            return default;
        }
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

    public class AvlTree<TK, TV> : BinarySearchTree<TK, TV> where TK : IComparable<TK>
    {
        int H(BinaryNode<(TK, TV)> n) => n?.Height ?? 0;
        int B(BinaryNode<(TK, TV)> n) => n == null ? 0 : H(n.Left) - H(n.Right);
        void U(BinaryNode<(TK, TV)> n) { if (n != null) n.Height = Math.Max(H(n.Left), H(n.Right)) + 1; }
        BinaryNode<(TK, TV)> RL(BinaryNode<(TK, TV)> y)
        { var x = y.Left; var T2 = x.Right; x.Right = y; y.Left = T2; U(y); U(x); return x; }
        BinaryNode<(TK, TV)> RR(BinaryNode<(TK, TV)> x)
        { var y = x.Right; var T2 = y.Left; y.Left = x; x.Right = T2; U(x); U(y); return y; }
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

    public class RedBlackTree<TK, TV> : BinarySearchTree<TK, TV> where TK : IComparable<TK>
    {
        bool IsRed(BinaryNode<(TK, TV)> n) => n != null && n.Red;
        BinaryNode<(TK, TV)> RotateLeft(BinaryNode<(TK, TV)> h)
        { var x = h.Right; h.Right = x.Left; x.Left = h; x.Red = h.Red; h.Red = true; return x; }
        BinaryNode<(TK, TV)> RotateRight(BinaryNode<(TK, TV)> h)
        { var x = h.Left; h.Left = x.Right; x.Right = h; x.Red = h.Red; h.Red = true; return x; }
        void FlipColors(BinaryNode<(TK, TV)> h)
        { h.Red = !h.Red; if (h.Left != null) h.Left.Red = !h.Left.Red; if (h.Right != null) h.Right.Red = !h.Right.Red; }
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
        public override void Insert(TK key, TV val)
        {
            Root = Insert(Root, key, val);
            if (Root != null) Root.Red = false;
        }
    }

    public class MinHeap<TK, TV> where TK : IComparable<TK>
    {
        class HNode { public TK Key; public TV Val; public HNode Parent; public HNode Left; public HNode Right; }
        HNode root; int count;
        public int Count => count;
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
        void BubbleUp(HNode n)
        {
            while (n.Parent != null && n.Key.CompareTo(n.Parent.Key) < 0)
            { Swap(n, n.Parent); n = n.Parent; }
        }
        void Swap(HNode a, HNode b)
        { var tk = a.Key; var tv = a.Val; a.Key = b.Key; a.Val = b.Val; b.Key = tk; b.Val = tv; }
        public (TK, TV) Peek() { if (root == null) return (default, default); return (root.Key, root.Val); }
        public IEnumerable<TV> InOrder()
        {
            foreach (var v in InOrder(root)) yield return v;
        }
        IEnumerable<TV> InOrder(HNode n)
        {
            if (n == null) yield break; foreach (var v in InOrder(n.Left)) yield return v; yield return n.Val; foreach (var v in InOrder(n.Right)) yield return v;
        }
    }

    public class Graph<T>
    {
        public class Edge { public GraphNode To; public int W; public Edge Next; }
        public class GraphNode
        {
            public T Val; public Edge First; public GraphNode Next;
            public GraphNode(T v) { Val = v; }
        }
        GraphNode head;
        public GraphNode AddNode(T v)
        {
            var n = new GraphNode(v) { Next = head }; head = n; return n;
        }
        public void AddUndirectedEdge(GraphNode a, GraphNode b, int w)
        {
            a.First = new Edge { To = b, W = w, Next = a.First };
            b.First = new Edge { To = a, W = w, Next = b.First };
        }
        public IEnumerable<T> Dfs(GraphNode start)
        {
            var visited = new HashSet<GraphNode>(); var stack = new Stack<GraphNode>(); stack.Push(start);
            while (stack.Count > 0)
            {
                var n = stack.Pop(); if (!visited.Add(n)) continue; yield return n.Val;
                var e = n.First; while (e != null) { stack.Push(e.To); e = e.Next; }
            }
        }
        public IEnumerable<(GraphNode, GraphNode, int)> PrimMst()
        {
            var resultHead = (GraphNode)null; var visited = new HashSet<GraphNode>();
            var p = head; if (p == null) yield break; visited.Add(p);
            while (true)
            {
                GraphNode bestU = null, bestV = null; int bestW = int.MaxValue; bool found = false;
                var u = head;
                while (u != null)
                {
                    if (visited.Contains(u))
                    {
                        var e = u.First;
                        while (e != null)
                        {
                            if (!visited.Contains(e.To) && e.W < bestW) { bestW = e.W; bestU = u; bestV = e.To; found = true; }
                            e = e.Next;
                        }
                    }
                    u = u.Next;
                }
                if (!found) yield break;
                visited.Add(bestV);
                yield return (bestU, bestV, bestW);
            }
        }
    }
}
