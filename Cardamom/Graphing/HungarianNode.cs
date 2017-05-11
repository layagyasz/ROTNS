using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    class HungarianNode : Node<object>
    {
        int _ID;
        bool _Mark;
        double _Potential;
        double _Slack;
        HungarianNode _Match;
        HungarianNode _SlackNode;
        HungarianNode _Parent;
        double[] _Neighbors;

        public int ID { get { return _ID; } }
        public bool Mark { get { return _Mark; } set { _Mark = value; } }
        public double Potential { get { return _Potential; } set { _Potential = value; } }
        public double Slack { get { return _Slack; } set { _Slack = value; } }
        public HungarianNode Match { get { return _Match; } set { _Match = value; } }
        public HungarianNode SlackNode { get { return _SlackNode; } set { _SlackNode = value; } }
        public HungarianNode Parent { get { return _Parent; } set { _Parent = value; } }
        public IEnumerable<KeyValuePair<int, double>> Neighbors { get { for (int i = 0; i < _Neighbors.Length; ++i) yield return new KeyValuePair<int, double>(i, _Neighbors[i]); } }

        public HungarianNode(object Value, int ID, int Size)
            : base(Value) { _Neighbors = new double[Size]; _ID = ID; }

        public void AddNeighbor(HungarianNode Node, double Weight, bool Bidirectional = true)
        {
            _Neighbors[((HungarianNode)Node).ID] = Weight;
            if (Bidirectional) Node.AddNeighbor(this, Weight, false);
        }

        public double GetWeight(HungarianNode Node)
        {
            return _Neighbors[Node.ID];
        }
    }
}
