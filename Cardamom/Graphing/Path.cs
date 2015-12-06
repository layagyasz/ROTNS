using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

using Cardamom.Utilities;

namespace Cardamom.Graphing
{
    public class Path<T> : Graph<T> where T : Pathable
    {
        List<T> _Path;

        double _Distance;
        bool _Complete;
        T _Destination;

        public int Count { get { return _Path.Count; } }
        public double Distance { get { return _Distance; } }
        public T Destination { get { return _Destination; } }
        public bool Complete { get { return _Complete; } }

        public T this[int i]
        {
            get { return _Path[i]; }
            set { _Path[i] = value; }
        }


        public Path(T Start, T Destination)
            : base()
        {
            _Path = new List<T>();

            Calculate(Start, Destination, DefaultFinishState);
        }
        public Path(T Start, T Destination, Func<T, T, bool> FinishState)
            : base()
        {
            _Path = new List<T>();

            Calculate(Start, Destination, FinishState);
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

        private void Calculate(T Start, T Destination, Func<T, T, bool> FinishState)
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
                IEnumerator<Pathable> it = Current.Value.Neighbors();
                while (it.MoveNext())
                {
                    if (it.Current != null)
                    {
                        double d = it.Current.DistanceTo(Current.Value) + Current.Distance;
                        bool h = HasNode((T)it.Current);
                        ANode<T> N = null;
                        if (h) N = (ANode<T>)GetNode((T)it.Current);
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
                            AddNode(new ANode<T>((T)it.Current));
                            N = (ANode<T>)GetNode((T)it.Current);
                        }
                        if (!inOpen && !inClosed && N.Value.Passable)
                        {
                            Open.Push(N, d);
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
            FindPath(Start, Current.Value);
            Clear();
        }

        void FindPath(T Start, T Destination)
        {
            T C = Destination;
            while (!C.Equals(Start))
            {
                _Distance += C.DistanceTo((T)((ANode<T>)_Nodes[C]).Parent);
                _Path.Add(C);
                C = (T)((ANode<T>)_Nodes[C]).Parent;
            }
            _Path.Add(Start);
            _Path.Reverse();
        }
    }
}
