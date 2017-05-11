using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Planar;

namespace Cardamom.Spatial
{
    class Camera
    {
        Vector4f _Origin;
        double _XRot;
        double _YRot;
        double _ZRot;
        float _FOV;
        SpatialTransformMatrix _Transform;
        Polygon _Bounds;

        public Vector4f Origin { get { return _Origin; } set { _Origin = value; } }
        public float FOV { get { return _FOV; } set { _FOV = value; } }

        public Camera(Polygon Bounds) { _Bounds = Bounds; }

        public void Rotate(double Angle, SpatialTransformMatrix.Axis Axis)
        {
            if(Axis == SpatialTransformMatrix.Axis.X) _XRot += Angle;
            else if (Axis == SpatialTransformMatrix.Axis.Y) _YRot += Angle;
            else _ZRot += Angle;
        }

        public Vector4f Transform(Vector4f Point)
        {
            Vector4f v = _Transform * Point;
            if (v.W > 0)
            {
                v.WDivide();
                v.S = _FOV / v.Z;
            }
            return v;
        }

        public Vector4f[] Transform(Face Face)
        {
            Vector4f[] V = new Vector4f[Face.Count];
            int i=0;
            foreach (Vector4f Vertex in Face) V[i++] = Transform(Vertex);
            return V;
        }

        public Face[] Transform(Model Model)
        {
            Face[] F = new Face[Model.Count];
            int i = 0;
            foreach (Face Face in Model) F[i++] = new Face(Transform(Face));
            return F;
        }

        public bool OnCamera(Vector4f Point)
        {
            return true;
        }

        public bool OnCamera(Face Face)
        {
            return true;
        }

        public void Update() { _Transform = CalculateMatrix(); }

        private SpatialTransformMatrix CalculateMatrix()
        {
            return new SpatialTransformMatrix().Translate(-_Origin).Rotate(_ZRot, SpatialTransformMatrix.Axis.Z).Rotate(_XRot, SpatialTransformMatrix.Axis.X).Project(_FOV).Translate(new Vector4f(_Bounds.Size.X / 2, _Bounds.Size.Y / 2, 0));
        }
    }
}
