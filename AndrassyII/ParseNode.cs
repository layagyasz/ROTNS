using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndrassyII
{
    class ParseNode<T> where T : Generator<T>
    {
        Operator<T> _Operator;
        ParseNode<T> _Parent;
        ParseNode<T> _Left;
        ParseNode<T> _Right;
        Generator<T> _Value;

        public ParseNode(string Source, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Sets)
        {
            for (int i = 0; i < Source.Length; ++i)
            {
                for (int j = 0; j < Operators.Count; ++j)
                {
                    if (Source[i] == Operators[j].Symbol)
                    {
                        string left = Source.Substring(0, i);
                        string right = Source.Substring(i + 1);
                        _Operator = Operators[j];
                        _Left = new ParseNode<T>(left, Sets);
                        _Right = new ParseNode<T>(right, Operators, Sets);
                        return;
                    }
                }
            }

            _Value = Sets[Source.Trim()];
        }

        private ParseNode(string Source, Dictionary<string, Generator<T>> Sets)
        {
            _Value = Sets[Source.Trim()];
        }

        public Generator<T> Traverse()
        {
            if (_Left != null && _Right != null)
            {
                return _Operator.Function.Invoke(_Left.Traverse(), _Right.Traverse());
            }
            else if (_Left == null && _Right != null) return _Right.Traverse();
            else if (_Left != null && _Right == null) return _Left.Traverse();
            else return _Value;
        }

        private void ReplaceChild(ParseNode<T> Old, ParseNode<T> New)
        {
            if (Old == _Left) _Left = New;
            else _Right = New;
        }

        public void Rebalance()
        {
            if (_Right != null)
            {
                if (_Right._Operator != null && _Right._Operator.Priority < _Operator.Priority) RebalanceLeft();
                _Right.Rebalance();
            }
        }

        private void RebalanceLeft()
        {
            ParseNode<T> OldRight = _Right;
            _Right = OldRight._Left;
            _Parent.ReplaceChild(this, _Right);
            _Parent = OldRight;
            _Parent._Left = this;
        }
    }
}
