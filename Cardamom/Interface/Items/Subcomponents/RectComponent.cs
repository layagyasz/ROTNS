using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

namespace Cardamom.Interface.Items.Subcomponents
{
    class RectComponent : Component
    {
        Vertex[] _Vertices;
        Vector2f _Position;

        public Vector2f Position { get { return _Position; } set { _Position = value; } }

        public RectComponent(Class Class)
        {
            int Height = (int)Class.GetAttributeWithDefault("height", (int)0);
            int Width = (int)Class.GetAttributeWithDefault("width", (int)0);
            Color[] _Colors = (Color[])Class.GetAttributeWithDefault("background-color", ClassLibrary.NullColors);
            _Vertices = new Vertex[]
            {
                new Vertex(new Vector2f(0,0), _Colors[0]),
                new Vertex(new Vector2f(Width, 0), _Colors[1]),
                new Vertex(new Vector2f(Width, Height), _Colors[2]),
                new Vertex(new Vector2f(0, Height), _Colors[3])
            };
        }

        public Polygon GetBoundingBox()
        {
            return new Polygon(new Vector2f[]
            {
                _Vertices[0].Position,
                _Vertices[1].Position,
                _Vertices[2].Position,
                _Vertices[3].Position
            });
        }

        public void Update(MouseController MouseController, KeyController KeyController, int DeltaT, PlanarTransformMatrix Transform)
        {
        }

        public void Draw(RenderTarget Target, PlanarTransformMatrix Transform)
        {
            Transform T = new PlanarTransformMatrix(Transform).Translate(_Position).ToTransform();
            Target.Draw(_Vertices, PrimitiveType.Quads, new RenderStates(T));
        }

        public void PerformTransitions(Dictionary<string, float> Transitions, Class From, Class To)
        {
            Color[] FromC = (Color[])From.GetAttributeWithDefault("background-color", ClassLibrary.NullColors);
            Color[] ToC = (Color[])To.GetAttributeWithDefault("background-color", ClassLibrary.NullColors);
            float T = (float)To.GetAttributeWithDefault(Transitions, "transition-background-color", "transition", 1);

            for (int i = 0; i < 4; ++i) _Vertices[i].Color = Cardamom.Utilities.ColorMath.BlendColors(FromC[i], ToC[i], T);
        }
    }
}
