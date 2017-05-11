using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    public class MaxCostAssignment<T, K>
    {
        private static readonly double _EPSILON = .00001;

        Dictionary<object, HungarianNode> _Nodes = new Dictionary<object, HungarianNode>();
        HungarianNode[] _Left;
        HungarianNode[] _Right;
        int _Rounds = 0;

        public K this[T Item] { get { return (K)_Nodes[Item].Match.Value; } }
        public T this[K Item] { get { return (T)_Nodes[Item].Match.Value; } }

        public MaxCostAssignment(IEnumerable<MaxCostAssignable> Left, IEnumerable<MaxCostAssignable> Right)
            : this((IEnumerable<T>)Left,
            (IEnumerable<K>)Right,
            (L, R) => ((MaxCostAssignable)L).DistanceTo((MaxCostAssignable)R),
            L => ((MaxCostAssignable)L).Neighbors() as IEnumerable<K>,
            R => ((MaxCostAssignable)R).Neighbors() as IEnumerable<T>) { }

        public MaxCostAssignment(IEnumerable<T> Left, IEnumerable<K> Right, Func<T, K, double> DistanceFunction, Func<T, IEnumerable<K>> LeftNeighbors, Func<K, IEnumerable<T>> RightNeighbors)
        {
            int NumLeft = Left.Count();
            int NumRight = Right.Count();
            _Left = new HungarianNode[NumLeft];
            _Right = new HungarianNode[NumRight];

            int LeftID = 0;
            foreach (T L in Left)
            {
                HungarianNode H = new HungarianNode(L, LeftID, NumRight);
                _Left[LeftID++] = H;
                _Nodes.Add(L, H);
            }
            int RightID = 0;
            foreach (K R in Right)
            {
                HungarianNode H = new HungarianNode(R, RightID, NumLeft);
                foreach (T N in RightNeighbors(R))
                {
                    H.AddNeighbor(_Nodes[N], DistanceFunction(N, R));
                }
                _Right[RightID++] = H;
                _Nodes.Add(R, H);
            }
            foreach (T L in Left)
            {
                foreach (K N in LeftNeighbors(L))
                {
                    _Nodes[L].AddNeighbor(_Nodes[N], DistanceFunction(L, N));
                }
            }

            Assign();
        }

        private void Assign()
        {
            _Rounds = 0;
            Array.ForEach(_Left, L => L.Potential = L.Neighbors.Max(N => N.Value));
            Array.ForEach(_Right, R => R.Potential = R.Neighbors.Max(N => N.Value));
            Queue<HungarianNode> Queue = new Queue<HungarianNode>();
            while (_Rounds < _Left.Length) Augment(Queue);
        }

        private void AddToTree(HungarianNode From, HungarianNode To)
        {
            To.Mark = true;
            To.Parent = From;
            foreach (HungarianNode Node in _Right)
            {
                if (To.Potential + Node.Potential - To.GetWeight(Node) < Node.Slack)
                {
                    Node.Slack = To.Potential + Node.Potential - To.GetWeight(Node);
                    Node.SlackNode = To;
                }
            }
        }

        private KeyValuePair<HungarianNode, HungarianNode> ExposePath(Queue<HungarianNode> Queue)
        {
            while (Queue.Count > 0)
            {
                HungarianNode Current = Queue.Dequeue();
                foreach (HungarianNode Node in _Right)
                {
                    if (Math.Abs(Current.GetWeight(Node) - Current.Potential - Node.Potential) < _EPSILON && !Node.Mark)
                    {
                        if (Node.Match == null) return new KeyValuePair<HungarianNode,HungarianNode>(Current, Node);
                        else
                        {
                            Node.Mark = true;
                            Queue.Enqueue(Node);
                            AddToTree(Node, Current);
                        }
                    }
                }
            }
            return new KeyValuePair<HungarianNode, HungarianNode>(null, null);
        }

        private KeyValuePair<HungarianNode, HungarianNode> ImproveLabeling(Queue<HungarianNode> Queue)
        {
            foreach (HungarianNode Node in _Right)
            {
                if (!Node.Mark && Math.Abs(Node.Slack) < _EPSILON)
                {
                    if (Node.Match == null) return new KeyValuePair<HungarianNode, HungarianNode>(Node.SlackNode, Node);
                    else
                    {
                        Node.Mark = true;
                        if (!Node.Match.Mark)
                        {
                            Queue.Enqueue(Node.Match);
                            AddToTree(Node.SlackNode, Node.Match);
                        }
                    }
                }
            }
            return new KeyValuePair<HungarianNode, HungarianNode>(null, null);
        }

        private void UpdateLabels()
        {
            double Delta = _Right.Min(R => !R.Mark ? R.Slack : double.MaxValue);
            Array.ForEach(_Left, L => L.Potential -= L.Mark ? Delta : 0);
            Array.ForEach(_Right, R => { R.Potential += R.Mark ? Delta : 0;  R.Slack -= !R.Mark ? Delta : 0;});
        }

        private void Augment(Queue<HungarianNode> Queue)
        {
            HungarianNode Root = null;
            foreach (HungarianNode Node in _Left)
            {
                if (Node.Match == null)
                {
                    Queue.Enqueue(Node);
                    Node.Parent = null;
                    Node.Mark = true;
                    Root = Node;
                }
            }

            foreach (HungarianNode Node in _Right)
            {
                Node.Slack = Root.Potential + Node.Potential - Root.GetWeight(Node);
                Node.SlackNode = Root;
            }

            KeyValuePair<HungarianNode, HungarianNode> Exposed = ExposePath(Queue);

            if (Exposed.Value == null)
            {
                UpdateLabels();
                Exposed = ImproveLabeling(Queue);
            }
            if (Exposed.Value != null)
            {
                _Rounds++;
                HungarianNode CurrentRight = Exposed.Value;
                HungarianNode CurrentLeft = Exposed.Key;
                while (CurrentLeft != null && CurrentRight != null)
                {
                    HungarianNode TempRight = CurrentLeft.Match;
                    CurrentRight.Match = CurrentLeft;
                    CurrentLeft.Match = CurrentRight;
                    CurrentRight = TempRight;
                    CurrentLeft = CurrentLeft.Parent;
                }
            }
        }
    }
}
