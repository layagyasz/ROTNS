using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    public abstract class Graph<T>
    {
        protected Dictionary<T, Node<T>> _Nodes;

        public Graph()
        {
            _Nodes = new Dictionary<T, Node<T>>();
        }

        public void AddNode(Node<T> Node)
        {
            _Nodes.Add(Node.Value, Node);
        }

        public Node<T> GetNode(T Value) { return _Nodes[Value]; }

        public bool HasNode(T Value) { return _Nodes.ContainsKey(Value); }

        protected void Clear() { _Nodes = null; }
    }
}
