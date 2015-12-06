using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;
using Cardamom.Planar;

namespace Cardamom.Interface.Items
{
    public class Pane : Container
    {
        PaneFrame _Frame;
        Polygon _DragBox;

        public Polygon DragBox { get { return _DragBox; } set { _DragBox = value; } }

        public Pane(string ClassName)
        {
            _Frame = new PaneFrame(ClassName);
        }
    }
}
