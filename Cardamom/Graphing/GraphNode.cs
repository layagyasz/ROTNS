using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;

namespace Cardamom.Graphing
{
    public class GraphNode<T> : Node<T>
    {
        List<Pair<float, GraphNode<T>>> _Neighbors = new List<Pair<float,GraphNode<T>>>();

        public IEnumerator<Pair<float,GraphNode<T>>> Neighbors { get { return _Neighbors.GetEnumerator(); } }

        public GraphNode(T Value) : base(Value) { }

        public void AddNeighbor(GraphNode<T> Neighbor, float Weight = 1) { _Neighbors.Add(new Pair<float, GraphNode<T>>(Weight, Neighbor)); }

        public void RemoveNeighbor(GraphNode<T> Neighbor)
        {
            Pair<float, GraphNode<T>> N = new Pair<float,GraphNode<T>>(0, null);
            foreach (Pair<float, GraphNode<T>> n in _Neighbors)
            {
                if (n.Second == Neighbor)
                {
                    N = n;
                    break;
                }
            }
            if(N.Second != null) _Neighbors.Remove(N);
        }
    }
}
