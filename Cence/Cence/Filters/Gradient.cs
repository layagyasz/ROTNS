using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cence.Gradients;

namespace Cence.Filters
{
    public class Gradient : Filter
    {
        public float Bias { get { return 0; } }
        public float Factor { get { return 1; } }

        public ChannelGradient X = ChannelGradient.Default;
        public ChannelGradient Y = ChannelGradient.Default;
        public ChannelGradient Z = ChannelGradient.Default;
        public ChannelGradient W = ChannelGradient.Flat;

        public ChannelGradient ColorChannels { set { X = Y = Z = value; } }

        bool _HSL;

        public Gradient(bool HSL = false)
        {
            _HSL = HSL;
        }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            return Evaluate(Image[X, Y]);
        }

        public FloatingColor Evaluate(FloatingColor C)
        {
            return Evaluate(C.Luminosity());
        }

        public FloatingColor Evaluate(float Value)
        {
            FloatingColor R = new FloatingColor()
            {
                R = X.Evaluate(Value),
                G = Y.Evaluate(Value),
                B = Z.Evaluate(Value),
                A = W.Evaluate(Value)
            };
            return (_HSL) ? R.MakeRGB() : R;
        }
    }
}
