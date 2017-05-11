using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

namespace Cardamom.Planar
{
    public class PlanarTransformMatrix
    {
        double e00 = 1;
        double e01 = 0;
        double e02 = 0;
        double e10 = 0;
        double e11 = 1;
        double e12 = 0;
        double e20 = 0;
        double e21 = 0;
        double e22 = 1;

        public PlanarTransformMatrix() { }

        public PlanarTransformMatrix(PlanarTransformMatrix Copy)
        {
            e00 = Copy.e00;
            e01 = Copy.e01;
            e02 = Copy.e02;
            e10 = Copy.e10;
            e11 = Copy.e11;
            e12 = Copy.e12;
            e20 = Copy.e20;
            e21 = Copy.e21;
            e22 = Copy.e22;
        }

        public PlanarTransformMatrix Rotate(double Angle)
        {
            double sin = Math.Sin(Angle);
            double cos = Math.Cos(Angle);

            double t = e00;
            e00 = e00 * cos + e01 * sin;
            e01 = t * -sin + e01 * cos;

            t = e10;
            e10 = e10 * cos + e11 * sin;
            e11 = t * -sin + e11 * cos;

            t = e20;
            e20 = e20 * cos + e21 * sin;
            e21 = t * -sin + e21 * cos;

            return this;
        }

        public PlanarTransformMatrix Scale(double ScaleX, double ScaleY)
        {
            e00 *= ScaleX;
            e01 *= ScaleX;
            e02 *= ScaleX;

            e10 *= ScaleY;
            e11 *= ScaleY;
            e12 *= ScaleY;

            return this;
        }

        public PlanarTransformMatrix Scale(double Constant)
        {
            return Scale(Constant, Constant);
        }

        public PlanarTransformMatrix Translate(Vector2f Vector)
        {
            e00 += Vector.X * e20;
            e01 += Vector.X * e21;
            e02 += Vector.X * e22;

            e10 += Vector.Y * e20;
            e11 += Vector.Y * e21;
            e12 += Vector.Y * e22;

            return this;
        }

        public static PlanarTransformMatrix operator *(PlanarTransformMatrix t, PlanarTransformMatrix v)
        {
            return new PlanarTransformMatrix()
            {
                e00 = t.e00 * v.e00 + t.e01 * v.e10 + t.e02 * v.e20,
                e01 = t.e00 * v.e01 + t.e01 * v.e11 + t.e02 * v.e21,
                e02 = t.e00 * v.e02 + t.e01 * v.e12 + t.e02 * v.e22,

                e10 = t.e10 * v.e00 + t.e11 * v.e10 + t.e12 * v.e20,
                e11 = t.e10 * v.e01 + t.e11 * v.e11 + t.e12 * v.e21,
                e12 = t.e10 * v.e02 + t.e11 * v.e12 + t.e12 * v.e22,

                e20 = t.e20 * v.e00 + t.e21 * v.e10 + t.e22 * v.e20,
                e21 = t.e20 * v.e01 + t.e21 * v.e11 + t.e22 * v.e21,
                e22 = t.e20 * v.e02 + t.e21 * v.e12 + t.e22 * v.e22
            };
        }

        public static Vector2f operator *(PlanarTransformMatrix t, Vector2f v)
        {
            return new Vector2f((float)(t.e00 * v.X + t.e01 * v.Y + t.e02), (float)(t.e10 * v.X + t.e11 * v.Y + t.e12));
        }

        public static Vector2f[] operator *(PlanarTransformMatrix t, Vector2f[] v)
        {
            Vector2f[] vr = new Vector2f[v.Length];
            for (int i = 0; i < v.Length; ++i)
            {
                vr[i] = t * v[i];
            }
            return vr;
        }

        public Transform ToTransform()
        {
            return new Transform((float)e00, (float)e01, (float)e02, (float)e10, (float)e11, (float)e12, (float)e20, (float)e21, (float)e22);
        }

        public override string ToString()
        {
            return String.Format("[[{0} {1} {2}][{3} {4} {5}][{6} {7} {8}]", e00, e01, e02, e10, e11, e12, e20, e21, e22);
        }
    }
}
