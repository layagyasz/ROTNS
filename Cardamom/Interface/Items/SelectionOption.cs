using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;

namespace Cardamom.Interface.Items
{
    public class SelectionOption<T> : StandardItem<T>
    {
        public SelectionOption(string ClassName)
            : base(ClassName, Series.Selectable) { }
    }
}
