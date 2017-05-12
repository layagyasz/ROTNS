using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class GradientTurbulent : Filter
    {
        float PeriodY;
        float PeriodX;
        float Turbulence;

        public float Bias { get { return 0; } }
        public float Factor { get { return 1; } }

        public GradientTurbulent(float PeriodX, float PeriodY, float Turbulence)
        {
            this.PeriodX = PeriodX;
            this.PeriodY = PeriodY;
            this.Turbulence = Turbulence;
        }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            float O = Y * PeriodY + X * PeriodX;
            return new FloatingColor()
            {
                R = 1 - O - Turbulence * Image[X, Y].R,
                G = 1 - O - Turbulence * Image[X, Y].G,
                B = 1 - O - Turbulence * Image[X, Y].B,
                A = Image[X, Y].A
            };
        }
    }
}
