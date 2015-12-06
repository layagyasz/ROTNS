using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;

using Cardamom.Interface.Items.Subcomponents;
using Cardamom.Planar;

namespace Cardamom.Interface.Items
{
    public abstract class GuiInput : ClassedGuiItem, Input
    {
        protected object _Value;

        public object Value { get { return _Value; } set { _Value = value; } }

        public GuiInput(string ClassName)
            : base(ClassName) { }

        public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, PlanarTransformMatrix Transform)
        {
            base.Update(MouseController, KeyController, DeltaT, Transform);
        }
    }
}
