﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using Cardamom.Utilities;
namespace Cardamom.Planar
{
	public class Segment
	{
		Vector2f _Point;
		Vector2f _End;
		Vector2f _Direction;
		float _Length;

		public Vector2f Point { get { return _Point; } }
		public Vector2f End { get { return _End; } }
		public Vector2f Direction { get { return _Direction; } }
		public float Length { get { return _Length; } }

		public Segment(Vector2f Start, Vector2f End)
		{
			_Point = Start;
			_End = End;
			_Length = (float)Math.Sqrt(Math.Pow(End.Y - Start.Y, 2) + Math.Pow(End.X - Start.X, 2));
			_Direction = (_End - _Point) / (float)_Length;
		}

		public Pair<bool, double> IntersectionDistance(Ray Ray)
		{
			double t2 = (Ray.DX * (_Point.Y - Ray.Point.Y) + Ray.DY
						 * (Ray.Point.X - _Point.X)) / (_Direction.X * Ray.DY - _Direction.Y * Ray.DX);
			double t1 = (_Point.X + _Direction.X * t2 - Ray.Point.X) / Ray.DX;

			if (t2 > _Length || t2 < 0 || t1 <= 0) return new Pair<bool, double>(false, 0);
			else return new Pair<bool, double>(true, t1);
		}

		public Vector2f Project(Vector2f Point)
		{
			double D = ProjectionDistance(Point);
			return new Vector2f((float)(_Point.X + _Direction.X * D), (float)(_Point.Y + _Direction.Y * D));
		}

		public double DistanceSquared(Vector2f Point)
		{
			Vector2f V = Project(Point);
			return (V.X - Point.X) * (V.X - Point.X) + (V.Y - Point.Y) * (V.Y - Point.Y);
		}

		internal double ProjectionDistance(Vector2f Point)
		{
			return ((_End.X - _Point.X) * (Point.X - _Point.X) + (_End.Y - _Point.Y) * (Point.Y - _Point.Y)) / _Length;
		}

		internal double ProjectDistanceSlope(Ray Ray)
		{
			return ((_End.X - _Point.X) * Ray.DX + (_End.Y - _Point.Y) * Ray.DY) / _Length;
		}

		public Pair<bool, Vector2f> Intersection(Segment Segment)
		{
			double t2 = (Segment._Direction.X * (_Point.Y - Segment.Point.Y) + Segment._Direction.Y
						 * (Segment.Point.X - _Point.X))
				/ (_Direction.X * Segment._Direction.Y - _Direction.Y * Segment._Direction.X);
			double t1 = (_Point.X + _Direction.X * t2 - Segment.Point.X) / Segment._Direction.X;

			if (t2 < _Length && t2 > 0 && t1 < Segment._Length && t1 > 0)
				return new Pair<bool, Vector2f>(
					true, new Vector2f((float)(t2 * _Direction.X + _Point.X), (float)(t2 * _Direction.Y + _Point.Y)));
			else return new Pair<bool, Vector2f>(false, new Vector2f(0, 0));
		}

		public Pair<bool, double> IntersectionDistance(Segment Segment)
		{
			double t2 = (Segment._Direction.X * (_Point.Y - Segment.Point.Y) + Segment._Direction.Y
						 * (Segment.Point.X - _Point.X))
				/ (_Direction.X * Segment._Direction.Y - _Direction.Y * Segment._Direction.X);
			double t1 = (_Point.X + _Direction.X * t2 - Segment.Point.X) / Segment._Direction.X;

			if (t2 < _Length && t2 > 0 && t1 < Segment._Length && t1 > 0) return new Pair<bool, double>(true, t2);
			else return new Pair<bool, double>(false, 0);
		}

		public static Segment operator *(PlanarTransformMatrix Transform, Segment Segment)
		{
			return new Segment(Transform * Segment._Point, Transform * Segment._End);
		}

		public static Segment operator +(Segment Segment, Vector2f Shift)
		{
			return new Segment(Segment._Point + Shift, Segment._End + Shift);
		}
	}
}
