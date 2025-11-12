using System;
using System.Collections.Generic;
using System.Collections;

namespace Municiple_Project_st10259527.Services.DataStructures
{
    public class BinaryNode<T>
    {
        public T Value;
        public BinaryNode<T> Left;
        public BinaryNode<T> Right;
        public int Height;
        public bool Red;
        public BinaryNode(T v) { Value = v; Height = 1; }
    }

    public class Binary<T> : IEnumerable<T>
    {
        public BinaryNode<T> Root { get; private set; }

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

        public IEnumerable<T> InOrder()
        {
            foreach (var v in InOrder(Root)) yield return v;
        }

        IEnumerable<T> InOrder(BinaryNode<T> node)
        {
            if (node == null) yield break;
            foreach (var v in InOrder(node.Left)) yield return v;
            yield return node.Value;
            foreach (var v in InOrder(node.Right)) yield return v;
        }

        public IEnumerable<T> PreOrder()
        {
            foreach (var v in PreOrder(Root)) yield return v;
        }

        IEnumerable<T> PreOrder(BinaryNode<T> node)
        {
            if (node == null) yield break;
            yield return node.Value;
            foreach (var v in PreOrder(node.Left)) yield return v;
            foreach (var v in PreOrder(node.Right)) yield return v;
        }

        public IEnumerable<T> PostOrder()
        {
            foreach (var v in PostOrder(Root)) yield return v;
        }

        IEnumerable<T> PostOrder(BinaryNode<T> node)
        {
            if (node == null) yield break;
            foreach (var v in PostOrder(node.Left)) yield return v;
            foreach (var v in PostOrder(node.Right)) yield return v;
            yield return node.Value;
        }

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

        public IEnumerator<T> GetEnumerator() { return LevelOrder().GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
