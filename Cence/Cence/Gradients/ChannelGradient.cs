using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Gradients
{
    public class ChannelGradient
    {
        public static readonly ChannelGradient Default = new ChannelGradient(
            new string[] { "linear" },
            new float[] { 0f, 1f },
            new float[] { 1f });

        public static readonly ChannelGradient Flat = new ChannelGradient(
            new string[] { "linear" },
            new float[] { 1f, 1f },
            new float[] { 1f });

        WeightedVector<ChannelGradientSegment> _Segments = new WeightedVector<ChannelGradientSegment>();

        public ChannelGradient() { }

        public ChannelGradient(string[] BlenderNames, float[] Targets, float[] Limits)
        {
            float O = 0;
            for (int i = 0; i < Limits.Length; ++i)
            {
                ChannelGradientSegment S = new ChannelGradientSegment(BlenderNames[i], O, Limits[i] - O, Targets[i], Targets[i + 1]);
                _Segments.Add(Limits[i] - O, S);
                O += Limits[i];
            }
        }

        public void Add(ChannelGradientSegment Segment)
        {
            _Segments.Add(Segment.Size, Segment);
        }

        public float Evaluate(float T)
        {
            ChannelGradientSegment S = _Segments[T];
            return S.Evaluate(T);
        }
    }
}
