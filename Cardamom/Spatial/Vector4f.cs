using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Spatial
{
    public struct Vector4f
    {
        float _X;
        float _Y;
        float _Z;
        float _W;
        float _S;

        public float X { get { return _X; } set { _X = value; } }
        public float Y { get { return _Y; } set { _Y = value; } }
        public float Z { get { return _Z; } set { _Z = value; } }
        public float W { get { return _W; } set { _W = value; } }
        public float S { get { return _S; } set { _S = value; } }

        public Vector4f(float X, float Y, float Z)
        {
            _X = X;
            _Y = Y;
            _Z = Z;
            _W = 1;
            _S = 0;
        }

        public Vector4f(double X, double Y, double Z)
        {
            _X = (float)X;
            _Y = (float)Y;
            _Z = (float)Z;
            _W = 1;
            _S = 0;
        }

        public void WDivide()
        {
            _X = _X / _W;
            _Y = _Y / _W;
        }

        public Vector4f Normalize()
        {
            float t = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
            return new Vector4f(X / t, Y / t, Z / t);
        }

        public double Distance(Vector4f v)
        {
            return Math.Sqrt(Math.Pow(_X - v.X, 2) + Math.Pow(_Y - v.Y, 2) + Math.Pow(_Z - v.Z, 2));
        }

        public static Vector4f operator +(Vector4f v1, Vector4f v2)
        {
            return new Vector4f(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector4f operator -(Vector4f v1, Vector4f v2)
        {
            return new Vector4f(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector4f operator *(Vector4f v, float m)
        {
            return new Vector4f(v.X * m, v.Y * m, v.Z * m);
        }

        public static float operator *(Vector4f v1, Vector4f v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector4f operator -(Vector4f v)
        {
            return new Vector4f(-v.X, -v.Y, -v.Z);
        }

        public static bool operator ==(Vector4f v1, Vector4f v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }

        public static bool operator !=(Vector4f v1, Vector4f v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "[Vector4f] X=" + _X + " Y=" + _Y + " Z=" + _Z;
        }
    }
}
