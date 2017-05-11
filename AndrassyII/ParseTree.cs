using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndrassyII
{
    class ParseTree<T> where T : Generator<T>
    {
        ParseNode<T> _Head;

        public ParseTree(string Source, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Sets)
        {
            _Head = new ParseNode<T>(Source, Operators, Sets);
            _Head.Rebalance();
            _Head = _Head.GetRoot();
        }

        public Set<T> Traverse() { return (Set<T>)_Head.Traverse(); }

        public override string ToString()
        {
            return _Head.ToString();
        }
    }
}
