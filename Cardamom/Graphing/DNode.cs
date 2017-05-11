using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    class DNode<T> : ANode<T> where T : Pathable<T>
    {
        bool _Center;

        public bool Center { get { return _Center; } }

        public DNode(T Value) : base(Value) { }

        public void MakeCenter() { _Center = true; }
    }
}
