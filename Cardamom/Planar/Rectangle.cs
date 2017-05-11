using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

using SFML.Window;

namespace Cardamom.Planar
{
    public class Rectangle : Polygon
    {
        public Rectangle(Vector2f TopLeft, Vector2f Size)
            : base(CreateVectors(TopLeft, Size)) { }

        public Rectangle(ParseBlock Block)
            : base(Block) { }

        private static Vector2f[] CreateVectors(Vector2f TopLeft, Vector2f Size)
        {
            Vector2f[] Vertices = new Vector2f[]
            {
                TopLeft,
                new Vector2f(TopLeft.X + Size.X, TopLeft.Y),
                TopLeft + Size,
                new Vector2f(TopLeft.X, TopLeft.Y + Size.Y)
            };
            return Vertices;
        }

        public void SetSize(Vector2f Size)
        {
            InitVectors(CreateVectors(this[0].Point, Size));
        }
    }
}
