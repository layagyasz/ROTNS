using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public class Evaluator
    {
        public delegate double EvaluatorFunction(double U, double V, Vector3d Vector);

        public static double Gradient(double U, double V, Vector3d Vector)
        {
            return Vector.X * U + Vector.Y * V;
        }

        public static double Hill(double U, double V, Vector3d Vector)
        {
            return Vector.Z * .5;
        }

        public static double HillAndSlope(double U, double V, Vector3d Vector)
        {
            return Hill(U, V, Vector) + Math.Min(0, Gradient(U, V, Vector));
        }

        public static double Curvature(double U, double V, Vector3d Vector)
        {
            return Vector.X * U * U + Vector.Y * V * V + 2 * U * V * Vector.Z;
        }

        public static double DoublePlane(double U, double V, Vector3d Vector)
        {
            return Math.Min(U * Vector.X + V * Vector.Y, U * Vector.Y + V * Vector.Z);
        }

        public static double VerticalEdge(double U, double V, Vector3d Vector)
        {
            return Math.Min(U * Vector.X + V * Vector.Y + Vector.Z, -U * Vector.X - V * Vector.Y + Vector.Z);
        }

        public static double VerticalEdgeInverse(double U, double V, Vector3d Vector)
        {
            return Math.Max(U * Vector.X + V * Vector.Y + Vector.Z, -U * Vector.X - V * Vector.Y + Vector.Z);
        }

        public static double TriangularEdge(double U, double V, Vector3d Vector)
        {
            return Math.Min(Math.Max(-U * Vector.X - V * Vector.Y, -U * Vector.Y + V * Vector.X),  Math.Max(U * Vector.X + V * Vector.Y, U * Vector.Y - V * Vector.X));
        }

        public static double MonkeySaddle(double U, double V, Vector3d Vector)
        {
            return Vector.X * (U * U * U - 3 * U * V * V);
        }

        public static double Hyperbolic(double U, double V, Vector3d Vector)
        {
            return 1f / (30f + Math.Abs(U * Vector.X + V * Vector.Y));
        }

        public static double HyperbolicPlanes(double U, double V, Vector3d Vector)
        {
            double s = (-U * Vector.X - V * Vector.Y);
            double t = (-U * Vector.Y + V * Vector.X);
            double u = (U * Vector.X + V * Vector.Y);
            double v = (U * Vector.Y - V * Vector.X);
            return Math.Min(Math.Max(s * s * s, t * t *t), Math.Max(u * u * u, v * v * v));
        }

        public static double HyperbolicPlanesDisplaced(double U, double V, Vector3d Vector)
        {
            return HyperbolicPlanes(U + Vector.Y * .25, V + Vector.Z * .25, Vector);
        }

        public static double VerticalEdgeDisplaced(double U, double V, Vector3d Vector)
        {
            return VerticalEdge(U + .5, V + .5, Vector);
        }

        public static double VerticalEdgeInverseDisplaced(double U, double V, Vector3d Vector)
        {
            return VerticalEdgeInverse(U + .5, V + .5, Vector);
        }

        public static double Parabolic(double U, double V, Vector3d Vector)
        {
            return U * U * Vector.X + V * V * Vector.Y + 2 * U * V * Vector.Z;
        }

        public static double ParabolicInverse(double U, double V, Vector3d Vector)
        {
            return Vector.Z - U * U * Vector.X - V * V * Vector.Y - 2 * U * V * Vector.Z;
        }

        public static double ParabolicComposed(double U, double V, Vector3d Vector)
        {
            return Math.Max(Parabolic(U, V, Vector), ParabolicInverse(U, V, Vector));
        }

        public static double ParabolicDisplaced(double U, double V, Vector3d Vector)
        {
            return (U + Vector.X) * (U + Vector.Y) + (V + Vector.Z) * (V + Vector.X);
        }

        public static double CosineInverse(double U, double V, Vector3d Vector)
        {
            double cV = Math.Cos(V * Math.PI);
            double cU = Math.Cos(V * Math.PI);
            return Vector.Z - (1 + Vector.X * cU + Vector.Y * cV + Vector.Z * cU * cV) / 4;
        }

        public static double Cosine(double U, double V, Vector3d Vector)
        {
            double cV = Math.Cos(V * Math.PI);
            double cU = Math.Cos(V * Math.PI);
            return (1 + Vector.X * cU + Vector.Y * cV + Vector.Z * cU * cV) / 4;
        }

        public static double Rejection(double U, double V, Vector3d Vector)
        {
            double G = (Vector.X * U + Vector.Y * V);
            return (U - G * Vector.X) * (U - G * Vector.X) + (V - G * Vector.Y) * (V - G * Vector.Y);
        }

        public static double Divergence(double U, double V, Vector3d Vector)
        {
            //Console.WriteLine("{0} {1} / {2} {3}", U, V, Vector.X, Vector.Y);
            return (U / (Vector.X * 16 + 17)) + (V / (Vector.Y * 16 + 17));
        }

        public static double Curl(double U, double V, Vector3d Vector)
        {
            return (-V / (Vector.Z * 16 + 17)) + (U / (Vector.Z * 16 + 17)) + (V / (Vector.Z * 16 + 17) - U / (Vector.Y * 16 + 17));
        }
    }
}
