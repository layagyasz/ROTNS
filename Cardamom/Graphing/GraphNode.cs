using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    public class GraphNode<T> : Node<T>
    {
        List<GraphNode<T>> _Neighbors = new List<GraphNode<T>>();

        public GraphNode(T Value) : base(Value) { }

        public void AddNeighbor(GraphNode<T> Neighbor) { _Neighbors.Add(Neighbor); }

        public void RemoveNeighbor(GraphNode<T> Neighbor) { _Neighbors.Remove(Neighbor); }
    }
}
