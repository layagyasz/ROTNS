using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

using Cardamom.Interface;

namespace ROTNS.Model.Flags
{
    public class Flag : GuiItem
    {
        Vertex[] _Vertices;

        public override Vector2f Size { get { return _Vertices[0].Position - _Vertices[2].Position; } }

        public Flag(FlagTemplate Template, FlagColor[] Colors)
        {
            _Vertices = Template.MakeFlag(Colors);
        }

        public override void Draw(RenderTarget Target, Transform Transform)
        {
            Transform.Translate(Position);
            Transform.Scale(75, 50);
            
            RenderStates R = new RenderStates(Transform);
            Target.Draw(_Vertices, PrimitiveType.Quads, R);
        }
    }
}
