using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;

namespace Cardamom.Graphing
{
	public class GraphNode<T> : Node<T>
	{
		List<Pair<float, GraphNode<T>>> _Neighbors = new List<Pair<float, GraphNode<T>>>();

		public IEnumerable<Pair<float, GraphNode<T>>> Edges { get { return _Neighbors.AsEnumerable(); } }

		public GraphNode(T Value) : base(Value) { }

		public void AddNeighbor(GraphNode<T> Neighbor, float Weight = 1) { _Neighbors.Add(new Pair<float, GraphNode<T>>(Weight, Neighbor)); }

		public void RemoveNeighbor(GraphNode<T> Neighbor)
		{
			_Neighbors.RemoveAll(i => i.Second == Neighbor);
		}
	}
}
