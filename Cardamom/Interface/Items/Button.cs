using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;

namespace Cardamom.Interface.Items
{
    public class Button : StandardItem<object>
    {
        public Button(string ClassName)
            : base(ClassName, Series.NoFocus) { }
    }
}
