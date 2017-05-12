using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Posterize : Filter
    {
        int Layers = 256;

        public float Bias { get { return 0; } }
        public float Factor { get { return 1; } }

        public Posterize(int Layers)
        {
            this.Layers = Layers;
        }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            return new FloatingColor()
            {
                R = (int)(Image[X,Y].R * Layers) / (float)(Layers - 1),
                G = (int)(Image[X, Y].G * Layers) / (float)(Layers - 1),
                B = (int)(Image[X, Y].B * Layers) / (float)(Layers - 1),
                A = Image[X,Y].A
            };
        }
    }
}
