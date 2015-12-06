using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;

namespace Cardamom.Interface.Items
{
    public class Button : GuiConstruct
    {
        public Button(string ClassName)
            : base(ClassName)
        {
            RectComponent R = new RectComponent(_Class);
            _Box = R.GetBoundingBox();

            _Components = new Component[]
            {
                R
            };
        }
    }
}
