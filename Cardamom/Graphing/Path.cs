using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

using Cardamom.Utilities;

namespace Cardamom.Graphing
{
	public class Path<T> : Graph<T>
	{
		List<T> _Path = new List<T>();

		double _Distance;
		bool _Complete;
		T _Destination;

		public int Count { get { return _Path.Count; } }
		public double Distance { get { return _Distance; } }
		public T Destination { get { return _Destination; } }
		public bool Complete { get { return _Complete; } }

		public IEnumerable<T> Nodes
		{
			get
			{
				return _Path;
			}
		}

		public T this[int i]
		{
			get { return _Path[i]; }
			set { _Path[i] = value; }
		}

		public Path(Path<T> Copy)
		{
			_Path = Copy._Path.ToList();
			_Distance = Copy._Distance;
			_Complete = Copy._Complete;
			_Destination = Copy._Destination;
		}

		public Path(T Start, T Destination,
			Func<T, bool> PassableFunction,
			Func<T, T, double> DistanceFunction,
			Func<T, T, double> HeuristicFunction,
			Func<T, IEnumerable<T>> NeighborFunction,
			Func<T, T, bool> FinishState)
			: base()
		{
			Calculate(Start, Destination,
					PassableFunction,
					DistanceFunction,
					HeuristicFunction,
					NeighborFunction,
					FinishState);
		}

		public Path(Pathable<T> Start, Pathable<T> Destination, Func<T, T, bool> FinishState)
			: this((T)Start, (T)Destination,
					P => ((Pathable<T>)P).Passable,
					(P1, P2) => ((Pathable<T>)P1).DistanceTo(P2),
					(P1, P2) => ((Pathable<T>)P1).HeuristicDistanceTo(P2),
					P => ((Pathable<T>)P).Neighbors(),
					FinishState)
		{ }


		public Path(Pathable<T> Start, Pathable<T> Destination)
			: this(Start, Destination, DefaultFinishState) { }

		public Path(IEnumerable<T> Path, Func<T, T, double> DistanceFunction)
		{
			_Path = Path.ToList();
			CalculateDistance(DistanceFunction);
		}

		public void Add(T Node, double Distance)
		{
			_Destination = Node;
			_Complete = false;
			_Distance += Distance;
			_Path.Add(Node);
		}

		public T Pop()
		{
			if (_Path.Count > 0)
			{
				T R = _Path[0];
				_Path.RemoveAt(0);
				return R;
			}
			else return default(T);
		}

		public static bool DefaultFinishState(T Current, T Destination)
		{
			return Current.Equals(Destination);
		}

		private void Calculate(
			T Start,
			T Destination,
			Func<T, bool> PassableFunction,
			Func<T, T, double> DistanceFunction,
			Func<T, T, double> HeuristicFunction,
			Func<T, IEnumerable<T>> NeighborFunction,
			Func<T, T, bool> FinishState)
		{
			AddNode(new ANode<T>(Start));
			PriorityQueue<ANode<T>, double> Open = new PriorityQueue<ANode<T>, double>();
			HashSet<ANode<T>> OpenCheck = new HashSet<ANode<T>>();
			OpenCheck.Add((ANode<T>)GetNode(Start));
			Open.Push((ANode<T>)GetNode(Start), 0);
			HashSet<ANode<T>> Closed = new HashSet<ANode<T>>();
			ANode<T> Current = Open.Peek();
			while (Open.Count > 0 && !FinishState(Open.Peek().Value, Destination))
			{
				Current = Open.Pop();
				OpenCheck.Remove(Current);
				Closed.Add(Current);
				foreach (T C in NeighborFunction(Current.Value))
				{
					if (C != null)
					{
						double d = DistanceFunction(Current.Value, C) + Current.Distance;
						bool h = HasNode((T)C);
						ANode<T> N = null;
						if (h) N = (ANode<T>)GetNode((T)C);
						bool inOpen = h && OpenCheck.Contains(N);
						bool inClosed = h && Closed.Contains(N);
						if (inOpen && N.Distance > d)
						{
							Open.Remove(N);
							OpenCheck.Remove(N);
							inOpen = false;
						}
						if (inClosed && N.Distance > d)
						{
							Closed.Remove(N);
							inClosed = false;
						}
						if (N == null)
						{
							AddNode(new ANode<T>((T)C));
							N = (ANode<T>)GetNode((T)C);
						}
						if (!inOpen && !inClosed && PassableFunction(N.Value))
						{
							Open.Push(N, d + HeuristicFunction(C, Destination));
							OpenCheck.Add(N);
							N.Update(d, Current.Value);
						}
					}
				}
			}
			if (Open.Count > 0) Current = Open.Peek();
			if (!FinishState(Current.Value, Destination)) _Complete = false;
			else _Complete = true;
			_Destination = Current.Value;
			FindPath(Start, Current.Value, DistanceFunction);
			Clear();
		}

		void FindPath(T Start, T Destination, Func<T, T, double> DistanceFunction)
		{
			T C = Destination;
			while (!C.Equals(Start))
			{
				_Path.Add(C);
				C = (T)((ANode<T>)_Nodes[C]).Parent;
			}
			_Path.Add(Start);
			_Path.Reverse();
			CalculateDistance(DistanceFunction);
		}

		void CalculateDistance(Func<T, T, double> DistanceFunction)
		{
			for (int i = 0; i < _Path.Count - 1; ++i)
			{
				_Distance += DistanceFunction(_Path[i], _Path[i + 1]);
			}
		}
	}
}
