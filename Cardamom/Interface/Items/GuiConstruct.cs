using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;
using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

namespace Cardamom.Interface.Items
{
    public abstract class GuiConstruct : GuiInput
    {
        protected Component[] _Components;
        protected Polygon _Box;
        private Polygon _TransformedBox;

        public GuiConstruct(string ClassName)
            : base(ClassName) { }

        public override bool IsCollision(Vector2f Point)
        {
            return _TransformedBox.ContainsPoint(Point);
        }

        public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, PlanarTransformMatrix Transform)
        {
            PlanarTransformMatrix T = new PlanarTransformMatrix(Transform).Translate(Position);
            _TransformedBox = T * _Box;
            base.Update(MouseController, KeyController, DeltaT, T);
            foreach (Component Component in _Components) Component.Update(MouseController, KeyController, DeltaT, T);
        }

        public override void Draw(RenderTarget Target, PlanarTransformMatrix Transform)
        {
            PlanarTransformMatrix T = new PlanarTransformMatrix(Transform).Translate(Position);
            foreach (Component Component in _Components) Component.Draw(Target, T);
        }

        public override void PerformTransitions(Dictionary<string, float> Transitions)
        {
            foreach (Component Component in _Components) Component.PerformTransitions(Transitions, _Class[_LastMode], _Class[_Mode]);
        }
    }
}
