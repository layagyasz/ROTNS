using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;

namespace Cardamom.Interface.Items
{
    class SliderKnob: GuiConstruct<object>
    {
        internal SliderKnob(string ClassName)
            : base(ClassName, Series.NoFocus)
        {
            RectComponent Back = new RectComponent(_Class);
            _Box = Back.GetBoundingBox();
            _Components = new Component[] { Back };
        }
    }
}
