using System;
using System.Collections.Generic;
using Cardamom.Utilities;

namespace Cardamom.Graphing
{
	public class Field<T> : Graph<T> where T : Pathable<T>
	{
		private Func<T, T, double> _DistanceFunction;
		private double _MaxDistance;

		public Field(T Start, double MaxDistance)
			: this(Start, MaxDistance, (i, j) => i.DistanceTo(j)) { }

		public Field(T Start, double MaxDistance, Func<T, T, double> DistanceFunction)
		{
			_DistanceFunction = DistanceFunction;
			_MaxDistance = MaxDistance;
			Resolve(Start);
		}

		public IEnumerable<Tuple<T, T, double>> GetReachableNodes()
		{
			foreach (var n in _Nodes)
			{
				if (((DNode<T>)n.Value).Distance <= _MaxDistance)
				{
					yield return new Tuple<T, T, double>(
						n.Value.Value,
						(T)((DNode<T>)n.Value).Parent,
						((DNode<T>)n.Value).Distance);
				}
			}
		}

		public Path<T> GetPathTo(T Final)
		{
			List<T> path = new List<T>();
			T C = Final;
			while (C != null)
			{
				path.Add(C);
				C = (T)((DNode<T>)_Nodes[C]).Parent;
			}
			path.Reverse();
			return new Path<T>(path, _DistanceFunction);
		}

		private void Resolve(T Start)
		{
			PriorityQueue<DNode<T>, double> open = new PriorityQueue<DNode<T>, double>();

			AddNode(new DNode<T>(Start));
			((DNode<T>)GetNode(Start)).Update(0, null);
			((DNode<T>)GetNode(Start)).MakeCenter();
			open.Push((DNode<T>)GetNode(Start), 0);

			HashSet<DNode<T>> Closed = new HashSet<DNode<T>>();
			while (open.Count > 0)
			{
				DNode<T> Current = open.Pop();
				if (Closed.Contains(Current) && open.Count > 0) Current = open.Pop();

				foreach (T Neighbor in Current.Value.Neighbors())
				{
					if (Neighbor != null && Neighbor.Passable)
					{
						double d = Current.Distance + _DistanceFunction(Current.Value, Neighbor);
						bool h = HasNode((T)Neighbor);
						DNode<T> N = null;
						if (h) N = (DNode<T>)GetNode((T)Neighbor);
						else
						{
							AddNode(new DNode<T>((T)Neighbor));
							N = (DNode<T>)GetNode((T)Neighbor);
							N.Update(Double.MaxValue, Current.Value);
						}
						if (N.Distance > d)
						{
							N.Update(d, Current.Value);
							if (d <= _MaxDistance) open.Push(N, d);
						}
					}
				}
			}
		}
	}
}
