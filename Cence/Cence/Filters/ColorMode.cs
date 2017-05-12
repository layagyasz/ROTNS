using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    class ColorMode : Filter
    {
        bool _HSL2RGB;

        public float Bias { get { return .5f; } }
        public float Factor { get { return 1; } }

        public ColorMode(bool HSL2RGB = true)
        {
            _HSL2RGB = HSL2RGB;
        }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            return _HSL2RGB ? Image[X, Y].MakeRGB() : Image[X,Y].MakeHSL();
        }
    }
}
