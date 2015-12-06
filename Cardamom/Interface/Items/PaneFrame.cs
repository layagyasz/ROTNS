using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;

namespace Cardamom.Interface.Items
{
    class PaneFrame : GuiConstruct
    {
        public PaneFrame(string ClassName)
            : base(ClassName)
        {
            _Components = new Component[]
            {
                new RectComponent(_Class)
            };
        }
    }
}
