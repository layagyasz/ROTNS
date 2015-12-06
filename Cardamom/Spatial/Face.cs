using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Spatial
{
    class Face : IEnumerable<Vector4f>
    {
        Vector4f[] _Vertices;

        public int Count { get { return _Vertices.Length; } }

        public IEnumerator<Vector4f> GetEnumerator() { return (IEnumerator<Vector4f>)_Vertices.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Face(Vector4f[] Vertices) { _Vertices = Vertices; }

        public Vector4f SurfaceNormal()
        {
            Vector4f N = new Vector4f();
            for (int i = 0; i < _Vertices.Length; ++i)
            {
                Vector4f Current = _Vertices[i];
                Vector4f Next = _Vertices[(i + 1) % _Vertices.Length];
                N.X += (Current.Y - Next.Y) * (Current.Z + Next.Z);
                N.Y += (Current.Z - Next.Z) * (Current.X + Next.X);
                N.Z += (Current.X - Next.X) * (Current.Y + Next.Y);
            }
            return N.Normalize();
        }
    }
}
