using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;

namespace Cardamom.Utilities
{
    public class CardinalSpline
    {
        List<Vector2f> _Points = new List<Vector2f>();
        double _Tension;

        public List<Vector2f> Points { get { return _Points; } }
        public double Tension { get { return _Tension; } set { _Tension = value; } }

        public Vector2f GetPoint(double Progression)
        {
            double ProgressionPerSegment = 1 / (_Points.Count - 1);
            int Segment = (int)(Progression * (_Points.Count - 1));
            float SegmentProgression = (float)(Progression * (Points.Count - 1) - Segment);
            Vector2f Left = Segment > 0 ? _Points[Segment - 1] : _Points[Segment];
            Vector2f Center = _Points[Segment];
            Vector2f Right = Segment < _Points.Count - 1 ? _Points[Segment + 1] : Center;
            Vector2f FarRight = Segment < _Points.Count - 2 ? _Points[Segment + 2] : Right;

            Vector2f MLeft = (float)(1 - _Tension) * (Right - Left) *.5f;
            Vector2f MRight = (float)(1 - _Tension) * (FarRight - Center) *.5f;
            return (1 + SegmentProgression * SegmentProgression * (-3 + SegmentProgression * 2)) * Center +
                (SegmentProgression * (1 + SegmentProgression * (-2 + SegmentProgression))) * MLeft +
                (SegmentProgression * SegmentProgression * (3 - 2 * SegmentProgression)) * Right +
                (SegmentProgression * SegmentProgression * (SegmentProgression - 1)) * MRight;
        }
    }
}
