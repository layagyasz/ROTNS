using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using Cardamom.Serialization;

namespace Cardamom.Planar
{
    public class Polygon : IEnumerable<Segment>
    {
        Segment[] _Segments;
        Vector2f _Size;

        public IEnumerator<Segment> GetEnumerator() { foreach (Segment S in _Segments) yield return S; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Vector2f Size { get { return _Size; } }
        public int Length { get { return _Segments.Length; } }
        public List<Segment> Segments { get { return _Segments.ToList(); } }

        public Segment this[int Index] { get { return _Segments[Index]; } }

        public Polygon(Vector2f[] Vertices)
        {
            InitVectors(Vertices);
        }

        public Polygon(ParseBlock Block)
        {
            InitVectors(Cardamom.Interface.ClassLibrary.Instance.ParseVector2fs(Block.String));
        }

        private void InitVectors(Vector2f[] Vertices)
        {
            _Segments = new Segment[Vertices.Length];

            float mX = float.MaxValue;
            float mY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            for (int i = 0; i < Vertices.Length; ++i)
            {
                _Segments[i] = new Segment(Vertices[i], Vertices[(i + 1) % Vertices.Length]);
                if (Vertices[i].X < mX) mX = Vertices[i].X;
                if (Vertices[i].X > maxX) maxX = Vertices[i].X;
                if (Vertices[i].Y < mY) mY = Vertices[i].Y;
                if (Vertices[i].Y > maxY) maxY = Vertices[i].Y;
            }

            _Size = new Vector2f(maxX - mX, maxY - mY);
        }

        public bool ContainsPoint(Vector2f Point)
        {
            bool c = false;
            int i, j = 0;
            for (i = 0, j = _Segments.Length - 1; i < _Segments.Length; j = i++)
            {
                if (((_Segments[i].Point.Y > Point.Y) != (_Segments[j].Point.Y > Point.Y)) &&
                 (Point.X < (_Segments[j].Point.X - _Segments[i].Point.X) * (Point.Y - _Segments[i].Point.Y) / (_Segments[j].Point.Y - _Segments[i].Point.Y) + _Segments[i].Point.X))
                    c = !c;
            }
            return c;
        }

        public float Area()
        {
            float a = 0;
            for (int i = 0; i < _Segments.Length - 1; i++)
            {
                a += _Segments[i].Point.X * _Segments[i + 1].Point.Y - _Segments[i + 1].Point.X * _Segments[i].Point.Y;
            }
            a += _Segments[_Segments.Length - 1].Point.X * _Segments[0].Point.Y - _Segments[0].Point.X * _Segments[_Segments.Length - 1].Point.Y;
            return Math.Abs(a) / 2;
        }

        public bool Intersects(Polygon Poly)
        {
            for (int i = 0; i < Poly.Length; ++i)
            {
                if (ContainsPoint(Poly[i].Point)) return true;
            }
            return Poly.Intersects(this);
        }

        public bool Intersects(Segment Segment)
        {
            if (ContainsPoint(Segment.Point) || ContainsPoint(Segment.End)) return true;
            foreach (Segment S in _Segments)
            {
                if (S.Intersection(Segment).First) return true;
            }
            return false;
        }

        public static Polygon operator *(PlanarTransformMatrix t, Polygon p)
        {
            Vector2f[] V = new Vector2f[p.Length];
            for (int i = 0; i < V.Length; ++i) V[i] = t * p[i].Point;
            return new Polygon(V);
        }
    }
}
