using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;

namespace Cardamom.Planar
{
    public struct Ray
    {
        double _Direction;
        Vector2f _Point;
        double _DX;
        double _DY;

        public double Direction { get { return _Direction; } }
        public Vector2f Point { get { return _Point; } }
        internal double DX { get { return _DX; } }
        internal double DY { get { return _DY; } }

        public Ray(Vector2f Point, double Direction)
        {
            _Direction = Direction;
            _Point = Point;
            _DX = Math.Cos(Direction);
            _DY = Math.Sin(Direction);
        }

        public Vector2f ToVector2f(double Length)
        {
            return new Vector2f((float)(Length * Math.Cos(_Direction) + _Point.X), (float)(Length * Math.Sin(_Direction) + _Point.Y));
        }

        public static Ray operator +(Ray Ray, Vector2f Point)
        {
            return new Ray(Point + Ray._Point, Ray._Direction);
        }

        public static Ray operator -(Ray Ray)
        {
            return new Ray(Ray._Point, -Ray._Direction);
        }
    }
}
