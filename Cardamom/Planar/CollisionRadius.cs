using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using Cardamom.Utilities;
using Cardamom.Serialization;

namespace Cardamom.Planar
{
	public class CollisionRadius : Collision
	{
		private enum Attribute { CENTER, RADIUS };

		double _Radius;
		Vector2f _Center;

		public double Radius { get { return _Radius; } }
		public Vector2f Center { get { return _Center; } }

		public CollisionRadius(Vector2f Center, double Radius)
		{
			_Radius = Radius;
			_Center = Center;
		}

		public CollisionRadius(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			_Center = (Vector2f)attributes[(int)Attribute.CENTER];
			_Radius = (double)attributes[(int)Attribute.RADIUS];
		}

		public double NearestCollision(Ray MoveRay, List<Segment> Segments, double Maximum)
		{
			MoveRay += _Center;
			double m = Maximum;
			foreach (Segment S in Segments)
			{
				double d = NearestCollision(MoveRay, S, Maximum);
				if (d < m && d >= 0) m = d;
			}
			return m;
		}

		private double NearestCollision(Ray MoveRay, Segment Segment, double Maximum)
		{
			double F = Segment.ProjectionDistance(MoveRay.Point);
			Vector2f V = Segment.Project(MoveRay.Point);
			double dx = (V.X - MoveRay.Point.X);
			double dy = (V.Y - MoveRay.Point.Y);
			if (dx * dx + dy * dy > (_Radius + Maximum) * (_Radius + Maximum)) return Maximum;
			else
			{
				if (F > Segment.Length || F < 0) return NearestCollision(MoveRay, Segment.Point, Maximum);
				else
				{
					double Q = Segment.ProjectDistanceSlope(MoveRay);
					double A = -2 * Q * Segment.DY * MoveRay.DY + MoveRay.DY * MoveRay.DY + Q * Q * Segment.DY * Segment.DY -
						2 * Q * Segment.DX * MoveRay.DX + MoveRay.DX * MoveRay.DX + Q * Q * Segment.DX * Segment.DX;
					double B = -2 * F * MoveRay.DY * Segment.DY + 2 * MoveRay.Point.Y * MoveRay.DY - 2 * Segment.Point.Y * MoveRay.DY +
						2 * F * Q * Segment.DY * Segment.DY - 2 * MoveRay.Point.Y * Q * Segment.DY + 2 * Q * Segment.Point.Y * Segment.DY -
						2 * F * MoveRay.DX * Segment.DX + 2 * MoveRay.Point.X * MoveRay.DX - 2 * Segment.Point.X * MoveRay.DX +
						2 * F * Q * Segment.DX * Segment.DX - 2 * MoveRay.Point.X * Q * Segment.DX + 2 * Q * Segment.Point.X * Segment.DX;
					double C = F * F * Segment.DY * Segment.DY - 2 * F * MoveRay.Point.Y * Segment.DY + 2 * F * Segment.Point.Y * Segment.DY +
						MoveRay.Point.Y * MoveRay.Point.Y - 2 * MoveRay.Point.Y * Segment.Point.Y + Segment.Point.Y * Segment.Point.Y +
						F * F * Segment.DX * Segment.DX - 2 * F * MoveRay.Point.X * Segment.DX + 2 * F * Segment.Point.X * Segment.DX +
						MoveRay.Point.X * MoveRay.Point.X - 2 * MoveRay.Point.X * Segment.Point.X + Segment.Point.X * Segment.Point.X -
						_Radius * _Radius;

					Triplet<bool, double, double> I = Quadratic(A, B, C);
					if (I.First) return Math.Min(I.Second, I.Third);
					else return NearestCollision(MoveRay, Segment.Point, Maximum);
				}
			}
		}

		private double NearestCollision(Ray MoveRay, Vector2f Point, double Maximum)
		{
			double dx = (Point.X - MoveRay.Point.X);
			double dy = (Point.Y - MoveRay.Point.Y);
			if (dx * dx + dy * dy > (_Radius + Maximum) * (_Radius + Maximum)) return Maximum;
			else
			{
				double A = MoveRay.DX * MoveRay.DX + MoveRay.DY * MoveRay.DY;
				double B = 2 * MoveRay.Point.Y * MoveRay.DY - 2 * Point.Y * MoveRay.DY +
					2 * MoveRay.Point.X * MoveRay.DX - 2 * Point.X * MoveRay.DX;
				double C = Point.Y * Point.Y - 2 * Point.Y * MoveRay.Point.Y + MoveRay.Point.Y * MoveRay.Point.Y +
					Point.X * Point.X - 2 * Point.X * MoveRay.Point.X + MoveRay.Point.X * MoveRay.Point.X - _Radius * _Radius;

				Triplet<bool, double, double> I = Quadratic(A, B, C);
				if (I.First)
				{
					if (I.Second > 0 && I.Second < I.Third) return I.Second;
					else return I.Third;
				}
				else return Maximum;
			}
		}

		private Triplet<bool, double, double> Quadratic(double A, double B, double C)
		{
			double det = B * B - 4 * A * C;
			if (det < 0) return new Triplet<bool, double, double>(false, 0, 0);
			else
			{
				det = Math.Sqrt(det);
				return new Triplet<bool, double, double>(true, (det - B) / (2 * A), (-det - B) / (2 * A));
			}
		}

		public Collision Multiply(PlanarTransformMatrix Transform)
		{
			return new CollisionRadius(Transform * _Center, _Radius);
		}
	}
}
