using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;
using Cardamom.Planar;

namespace Cardamom.Planar
{
    public interface Collision
    {
        double NearestCollision(Ray MoveRay, List<Segment> Segments, double Maximum);
        Collision Multiply(PlanarTransformMatrix Transform);
    }
}
