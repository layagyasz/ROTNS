using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Graphing;
using SFML.Window;

namespace Cardamom.Planar
{
    internal class VNode : GraphNode<Vector2f>
    {
        public VNode(Vector2f Value) : base(Value) { }

        public void SplitEdge(VNode Sibling, VNode Splitter)
        {
            Splitter.AddNeighbor(this);
            Splitter.AddNeighbor(Sibling);
            Sibling.RemoveNeighbor(this);
            RemoveNeighbor(Sibling);
        }
    }
}
