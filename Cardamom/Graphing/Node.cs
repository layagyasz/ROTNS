using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    public abstract class Node<T>
    {
        T _Value;
        public T Value { get { return _Value; } }

        public Node(T Value) { _Value = Value; }
    }
}
