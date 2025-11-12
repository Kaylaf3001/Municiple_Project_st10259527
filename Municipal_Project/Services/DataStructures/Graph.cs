using System;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Services.DataStructures
{
    public class Graph<T>
    {
        public class Edge { public GraphNode To; public int W; public Edge Next; }

        public class GraphNode
        {
            public T Val; public Edge First; public GraphNode Next;
            public GraphNode(T v) { Val = v; }
        }

        GraphNode head;

        public GraphNode AddNode(T value)
        {
            var node = new GraphNode(value) { Next = head }; head = node; return node;
        }

        public void AddUndirectedEdge(GraphNode nodeA, GraphNode nodeB, int weight)
        {
            nodeA.First = new Edge { To = nodeB, W = weight, Next = nodeA.First };
            nodeB.First = new Edge { To = nodeA, W = weight, Next = nodeB.First };
        }

        public IEnumerable<T> Dfs(GraphNode start)
        {
            var visited = new HashSet<GraphNode>(); var stack = new Stack<GraphNode>(); stack.Push(start);
            while (stack.Count > 0)
            {
                var current = stack.Pop(); if (!visited.Add(current)) continue; yield return current.Val;
                var edge = current.First; while (edge != null) { stack.Push(edge.To); edge = edge.Next; }
            }
        }

        public IEnumerable<T> Bfs(GraphNode start)
        {
            var visited = new HashSet<GraphNode>(); var q = new Queue<GraphNode>(); q.Enqueue(start);
            while (q.Count > 0)
            {
                var current = q.Dequeue(); if (!visited.Add(current)) continue; yield return current.Val;
                var edge = current.First; while (edge != null) { q.Enqueue(edge.To); edge = edge.Next; }
            }
        }

        public IEnumerable<(GraphNode To, int W)> Neighbors(GraphNode node)
        {
            var edge = node?.First; while (edge != null) { yield return (edge.To, edge.W); edge = edge.Next; }
        }

        public void AddOrUpdateUndirectedEdge(GraphNode fromNode, GraphNode toNode, int weight)
        {
            if (fromNode == null || toNode == null || ReferenceEquals(fromNode, toNode)) return;

            void Upsert(GraphNode from, GraphNode to)
            {
                var edge = from.First; Edge prev = null; while (edge != null)
                {
                    if (ReferenceEquals(edge.To, to))
                    {
                        if (weight < edge.W) edge.W = weight;
                        return;
                    }
                    prev = edge; edge = edge.Next;
                }
                from.First = new Edge { To = to, W = weight, Next = from.First };
            }

            Upsert(fromNode, toNode);
            Upsert(toNode, fromNode);
        }

        public IEnumerable<GraphNode> Nodes()
        {
            var node = head; while (node != null) { yield return node; node = node.Next; }
        }

        public IEnumerable<(GraphNode From, GraphNode To, int W)> Edges()
        {
            var node = head; while (node != null) { var edge = node.First; while (edge != null) { yield return (node, edge.To, edge.W); edge = edge.Next; } node = node.Next; }
        }

        public IEnumerable<(GraphNode, GraphNode, int)> PrimMst()
        {
            var visited = new HashSet<GraphNode>();
            var firstNode = head; if (firstNode == null) yield break; visited.Add(firstNode);

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
    }
}
