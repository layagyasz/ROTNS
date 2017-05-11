using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace Cardamom.Spatial
{
    class SpatialTransformMatrix
    {
        public enum Axis { X, Y, Z };

        float e00;
        float e01;
        float e02;
        float e03;
        float e10;
        float e11;
        float e12;
        float e13;
        float e20;
        float e21;
        float e22;
        float e23;
        float e30;
        float e31;
        float e32;
        float e33;

        public SpatialTransformMatrix() { }

        public void Reset()
        {
            e00 = 1;
            e01 = 0;
            e02 = 0;
            e03 = 0;
            e10 = 0;
            e11 = 1;
            e12 = 0;
            e13 = 0;
            e20 = 0;
            e21 = 0;
            e22 = 1;
            e23 = 0;
            e30 = 0;
            e31 = 0;
            e32 = 0;
            e33 = 1;
        }

        public void SynchronizeShader(Shader Shader)
        {
            Shader.SetParameter("viewMatrix_0", e00, e01, e02, e03);
            Shader.SetParameter("viewMatrix_1", e10, e11, e12, e13);
            Shader.SetParameter("viewMatrix_2", e20, e21, e22, e23);
            Shader.SetParameter("viewMatrix_3", e30, e31, e32, e33);
        }

        public SpatialTransformMatrix Project(float FOV)
        {
            e00 *= FOV;
            e01 *= FOV;
            e02 *= FOV;
            e03 *= FOV;

            e10 *= FOV;
            e11 *= FOV;
            e12 *= FOV;
            e13 *= FOV;

            e30 = e20;
            e31 = e21;
            e32 = e22;
            e33 = e23;

            return this;
        }

        public SpatialTransformMatrix Scale(float Const)
        {
            return Scale(new Vector4f(Const, Const, Const));
        }

        public SpatialTransformMatrix Scale(Vector4f Vector)
        {
            e00 *= Vector.X;
            e01 *= Vector.X;
            e02 *= Vector.X;
            e03 *= Vector.X;

            e10 *= Vector.Y;
            e11 *= Vector.Y;
            e12 *= Vector.Y;
            e13 *= Vector.Y;

            e20 *= Vector.Z;
            e21 *= Vector.Z;
            e22 *= Vector.Z;
            e23 *= Vector.Z;

            return this;
        }

        public SpatialTransformMatrix Translate(Vector4f Vector)
        {
            e00 += Vector.X * e30;
            e01 += Vector.X * e31;
            e02 += Vector.X * e32;
            e03 += Vector.X * e33;

            e10 += Vector.Y * e30;
            e11 += Vector.Y * e31;
            e12 += Vector.Y * e32;
            e13 += Vector.Y * e33;

            e20 += Vector.Z * e30;
            e21 += Vector.Z * e31;
            e22 += Vector.Z * e32;
            e23 += Vector.Z * e33;

            return this;
        }

        public SpatialTransformMatrix Rotate(double Angle, Axis Axis)
        {
            float sin = (float)Math.Sin(Angle);
            float cos = (float)Math.Cos(Angle);

            float te00, te01, te02, te10, te11, te12, te20, te21, te22;

            if (Axis == Axis.X)
            {
                    te00 = 1;
                    te01 = 0;
                    te02 = 0;
                    te10 = 0;
                    te11 = cos;
                    te12 = -sin;
                    te20 = 0;
                    te21 = sin;
                    te22 = cos;
            }
            else if (Axis == Axis.Y)
            {
                    te00 = cos;
                    te01 = 0;
                    te02 = sin;
                    te10 = 0;
                    te11 = 1;
                    te12 = 0;
                    te20 = -sin;
                    te21 = 0;
                    te22 = cos;
            }
            else
            {
                    te00 = cos;
                    te01 = -sin;
                    te02 = 0;
                    te10 = sin;
                    te11 = cos;
                    te12 = 0;
                    te20 = 0;
                    te21 = 0;
                    te22 = 1;
            }

            e00 = te00 * e00 + te01 * e10 + te02 * e20;
            e01 = te00 * e01 + te01 * e11 + te02 * e21;
            e02 = te00 * e02 + te01 * e12 + te02 * e22;
            e03 = te00 * e03 + te01 * e13 + te02 * e23;

            e10 = te10 * e00 + te11 * e10 + te12 * e20;
            e11 = te10 * e01 + te11 * e11 + te12 * e21;
            e12 = te10 * e02 + te11 * e12 + te12 * e22;
            e13 = te10 * e03 + te11 * e13 + te12 * e23;

            e20 = te20 * e00 + te21 * e10 + te22 * e20;
            e21 = te20 * e01 + te21 * e11 + te22 * e21;
            e22 = te20 * e02 + te21 * e12 + te22 * e22;
            e23 = te20 * e03 + te21 * e13 + te22 * e23;

            return this;
        }

        public static Vector4f operator *(SpatialTransformMatrix t, Vector4f v)
        {
            return new Vector4f()
            {
                X = v.X * t.e00 + v.Y * t.e01 + v.Z * t.e02 + v.W * t.e03,
                Y = v.X * t.e10 + v.Y * t.e11 + v.Z * t.e12 + v.W * t.e13,
                Z = v.X * t.e20 + v.Y * t.e21 + v.Z * t.e22 + v.W * t.e23,
                W = v.X * t.e30 + v.Y * t.e31 + v.Z * t.e32 + v.W * t.e33
            };
        }

        public static SpatialTransformMatrix operator *(SpatialTransformMatrix t, SpatialTransformMatrix v)
        {
            return new SpatialTransformMatrix()
            {
                e00 = t.e00 * v.e00 + t.e01 * v.e10 + t.e02 * v.e20 + t.e03 * v.e30,
                e01 = t.e00 * v.e01 + t.e01 * v.e11 + t.e02 * v.e21 + t.e03 * v.e31,
                e02 = t.e00 * v.e02 + t.e01 * v.e12 + t.e02 * v.e22 + t.e03 * v.e32,
                e03 = t.e00 * v.e03 + t.e01 * v.e13 + t.e02 * v.e23 + t.e03 * v.e33,

                e10 = t.e10 * v.e00 + t.e11 * v.e10 + t.e12 * v.e20 + t.e13 * v.e30,
                e11 = t.e10 * v.e01 + t.e11 * v.e11 + t.e12 * v.e21 + t.e13 * v.e31,
                e12 = t.e10 * v.e02 + t.e11 * v.e12 + t.e12 * v.e22 + t.e13 * v.e32,
                e13 = t.e10 * v.e03 + t.e11 * v.e13 + t.e12 * v.e23 + t.e13 * v.e33,

                e20 = t.e20 * v.e00 + t.e21 * v.e10 + t.e22 * v.e20 + t.e23 * v.e30,
                e21 = t.e20 * v.e01 + t.e21 * v.e11 + t.e22 * v.e21 + t.e23 * v.e31,
                e22 = t.e20 * v.e02 + t.e21 * v.e12 + t.e22 * v.e22 + t.e23 * v.e32,
                e23 = t.e20 * v.e03 + t.e21 * v.e13 + t.e22 * v.e23 + t.e23 * v.e33,

                e30 = t.e30 * v.e00 + t.e31 * v.e10 + t.e32 * v.e20 + t.e33 * v.e30,
                e31 = t.e30 * v.e01 + t.e31 * v.e11 + t.e32 * v.e21 + t.e33 * v.e31,
                e32 = t.e30 * v.e02 + t.e31 * v.e12 + t.e32 * v.e22 + t.e33 * v.e32,
                e33 = t.e30 * v.e03 + t.e31 * v.e13 + t.e32 * v.e23 + t.e33 * v.e33
            };
        }
    }
}
