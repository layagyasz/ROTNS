using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Interface.Items
{
    public class Select<T> : GuiSerialContainer<SelectionOption<T>>
    {
        public Select(string ClassString)
            : base(ClassString, true) { }
    }
}
