using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndrassyII
{
    class Operator<T> where T : Generator<T>
    {
        char _Operator;
        int _Priority;
        Func<Generator<T>, Generator<T>, Generator<T>> _Function;

        public char Symbol { get { return _Operator; } }
        public int Priority { get { return _Priority; } }
        public Func<Generator<T>, Generator<T>, Generator<T>> Function { get { return _Function; } }

        public Operator(char Operator, int Priority, Func<Generator<T>, Generator<T>, Generator<T>> Function)
        {
            _Operator = Operator;
            _Function = Function;
        }
    }
}
