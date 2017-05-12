using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Sobel : Filter
    {
        float _Scale;

        public float Bias { get { return 0; } }
        public float Factor { get { return 1; } }
        public float Scale { get { return _Scale; } }

        public Sobel(float Scale) { _Scale = Scale; }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            float x = _Scale * ((Image[X + 1, Y + 1] - Image[X - 1, Y + 1]) + 2 * (Image[X + 1,Y] - Image[X -1, Y]) + (Image[X+1, Y-1] - Image[X-1, Y-1])).R;
            float y = _Scale * ((Image[X - 1, Y - 1] - Image[X - 1, Y + 1]) + 2 * (Image[X, Y - 1] - Image[X, Y + 1]) + (Image[X + 1, Y - 1] - Image[X + 1, Y + 1])).R;
            float z = 1;

            float l = (float)Math.Sqrt(x * x + y * y + z);
            x /= l;
            y /= l;
            z /= l;

            return new FloatingColor(x / 2 + .5f, y / 2 + .5f, z / .5f);
        }
    }
}
