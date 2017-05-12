using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Gradients
{
    public class ChannelGradientSegment
    {
        Func<double, double> _Blender;
        float _From;
        float _To;
        float _Offset;
        float _Size;

        public float From { get { return _From; } }
        public float To { get { return _To; } }
        public float Offset { get { return _Offset; } }
        public float Size { get { return _Size; } }

        public ChannelGradientSegment(string Blender, float Offset, float Size, float From, float To)
        {
            _Blender = Blenders.Functions[Blender];
            _Offset = Offset;
            _Size = Size;
            _From = From;
            _To = To;
        }

        public float Evaluate(float T)
        {
            float a = (float)_Blender((T - _Offset) / _Size);
            return _From * a + _To * (1 - a);
        }
    }
}
