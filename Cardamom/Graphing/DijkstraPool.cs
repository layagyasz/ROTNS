using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;

namespace Cardamom.Graphing
{
    public class DijkstraPool<T> : Graph<T> where T : Pathable<T>
    {
        List<DijkstraRegion<T>> _Starts = new List<DijkstraRegion<T>>();

        public DijkstraPool()
            : base() { }

        public object GetParent(T Node) { return ((DNode<T>)GetNode(Node)).Parent; }

        public double GetDistance(T Node) { return ((DNode<T>)GetNode(Node)).Distance; }

        public bool Epicenter(T Node) { return HasNode(Node) && ((DNode<T>)GetNode(Node)).Center; }

        public void Drop(DijkstraRegion<T> Region)
        {
            _Starts.Add(Region);
        }

        public void Resolve()
        {
            PriorityQueue<DNode<T>, double> open = new PriorityQueue<DNode<T>, double>();
            foreach (DijkstraRegion<T> Start in _Starts)
            {
                if (!HasNode(Start.Center)) AddNode(new DNode<T>(Start.Center));
                ((DNode<T>)GetNode(Start.Center)).Update(Start.StartDistance, Start);
                ((DNode<T>)GetNode(Start.Center)).MakeCenter();
                open.Push((DNode<T>)GetNode(Start.Center), Start.StartDistance);
            }

            HashSet<DNode<T>> Closed = new HashSet<DNode<T>>();
            while (open.Count > 0)
            {
                DNode<T> Current = open.Pop();
                if (Closed.Contains(Current) && open.Count > 0) Current = open.Pop();
                if (open.Count == 0) break;
                
                IEnumerable<T> Neighbors = Current.Value.Neighbors();
                foreach (T Neighbor in Current.Value.Neighbors())
                {
                    if (Neighbor != null && Neighbor.Passable)
                    {
                        double d = Current.Distance + Current.Value.DistanceTo(Neighbor);
                        bool h = HasNode((T)Neighbor);
                        DNode<T> N = null;
                        if (h) N = (DNode<T>)GetNode((T)Neighbor);
                        else { AddNode(new DNode<T>((T)Neighbor)); N = (DNode<T>)GetNode((T)Neighbor); N.Update(Double.MaxValue, Current.Parent); }
                        if (N.Distance > d)
                        {
                            N.Update(d, Current.Parent);
                            open.Push(N, d);
                        }
                    }
                }
            }

            foreach (KeyValuePair<T, Node<T>> P in _Nodes)
            {
                ((DijkstraRegion<T>)((DNode<T>)P.Value).Parent).Add(P.Value.Value);
            }
        }
    }
}
