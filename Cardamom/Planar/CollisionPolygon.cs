using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using Cardamom.Serialization;
using Cardamom.Utilities;

namespace Cardamom.Planar
{
    public class CollisionPolygon : Polygon, Collision
    {
        public CollisionPolygon(Vector2f[] Vertices) : base(Vertices) { }
        public CollisionPolygon(ParseBlock Block) : base(Block) { }

        public double NearestCollision(Ray MoveRay, List<Segment> Segments, double Maximum)
        {
            double travel = Maximum;
            foreach (Segment S in Segments)
            {
                foreach (Segment C in this)
                {
                    Pair<bool, double> I = S.IntersectionDistance(MoveRay + C.Point);
                    if (I.First && I.Second < travel) travel = I.Second;

                    Ray CounterRay = -MoveRay;
                    I = C.IntersectionDistance(CounterRay + S.Point);
                    if (I.First && I.Second < travel) travel = I.Second;
                }
            }
            return travel;
        }

        public Collision Multiply(PlanarTransformMatrix Transform)
        {
            Vector2f[] V = new Vector2f[Length];
            for (int i = 0; i < V.Length; ++i) V[i] = Transform * this[i].Point;
            return new CollisionPolygon(V);
        }
    }
}
