using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public struct Pixel
    {
        int _X;
        int _Y;
        FloatingColor _Color;

        public int X { get { return _X; } }
        public int Y { get { return _Y; } }
        public FloatingColor Color { get { return _Color; } }

        public Pixel(int X, int Y, FloatingColor Color)
        {
            _X = X;
            _Y = Y;
            _Color = Color;
        }
    }
}
