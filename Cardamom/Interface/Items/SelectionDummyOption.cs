using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Interface.Items
{
    class SelectionDummyOption<T> : StandardItem<T>
    {
        public SelectionDummyOption(string ClassName)
            : base(ClassName, Series.Standard) { }

        public SelectionDummyOption(SelectionOption<T> Clone)
            : base(Clone.Class.Name, Series.Standard) { }
    }
}
