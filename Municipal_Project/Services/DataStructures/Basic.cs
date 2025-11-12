using System;
using System.Collections;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Services.DataStructures
{
    public class Node<T>
    {
        public T Value { get; set; }
        public Node<T> FirstChild { get; set; }
        public Node<T> NextSibling { get; set; }
        public Node(T value) { Value = value; }
    }

    public class Basic<T> : IEnumerable<T>
    {
        public Node<T> Root { get; private set; }

        public void SetRoot(T value) { Root = new Node<T>(value); }

        public void AddChild(Node<T> parent, T value)
        {
            if (parent.FirstChild == null) parent.FirstChild = new Node<T>(value);
            else
            {
                var currentSibling = parent.FirstChild;
                while (currentSibling.NextSibling != null) currentSibling = currentSibling.NextSibling;
                currentSibling.NextSibling = new Node<T>(value);
            }
        }

        public IEnumerator<T> GetEnumerator() { return Traverse(Root).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        IEnumerable<T> Traverse(Node<T> node)
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
}
