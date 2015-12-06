using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

using Cardamom.Planar;

namespace Cardamom.Interface.Items.Subcomponents
{
    public class BorderComponent : Component
    {
        public BorderComponent(Vector2f[] Corners, Class Class)
        {
            byte[] Widths = (byte[])Class.GetAttributeWithDefault("border-width", null);
            if (Widths != null)
            {
                for (int i = 0; i < Corners.Length; ++i)
                {

                }
            }
        }

        public void Update(MouseController MouseController, KeyController KeyController, int DeltaT, PlanarTransformMatrix Transform)
        {
        }

        public void Draw(RenderTarget Target, PlanarTransformMatrix Transform)
        {
        }

        public void PerformTransitions(Dictionary<string, float> Transitions, Class From, Class To)
        {
        }
    }
}
