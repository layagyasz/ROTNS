using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public struct Vector3d
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3d(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Vector3d Normalize()
        {
            double SumSquared = X * X + Y * Y + Z * Z;
            return new Vector3d(X / SumSquared, Y / SumSquared, Z / SumSquared);
        }
    }
}
