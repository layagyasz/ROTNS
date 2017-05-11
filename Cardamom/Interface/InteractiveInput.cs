using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Interface
{
    public abstract class InteractiveInput<T> : Interactive
    {
        public EventHandler OnChange;

        protected T _Value;

        public T Value { get { return _Value; } set { _Value = value; } }
    }
}
