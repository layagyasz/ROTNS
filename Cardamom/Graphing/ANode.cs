using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    class ANode<T> : Node<T>
    {
        double _Distance;
        object _Parent;

        public double Distance { get { return _Distance; } }
        public object Parent { get { return _Parent; } }

        public ANode(T Value) : base(Value) { }

        public void Update(double Distance, object Parent)
        {
            _Distance = Distance;
            _Parent = Parent;
        }
    }
}
