using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Erode : TranscendentalFilter
    {
        int _Drops;
        int _Steps;
        Brush _Brush;

        private static readonly Pair<int, int>[] _NEIGHBORS = new Pair<int, int>[]
        {
            new Pair<int, int>(-1,-1),
            new Pair<int, int>(-1,0),
            new Pair<int, int>(-1,1),
            new Pair<int, int>(0,-1),
            new Pair<int, int>(0,1),
            new Pair<int, int>(1,-1),
            new Pair<int, int>(1,0),
            new Pair<int, int>(1,1)
        };

        public Erode(int Steps, int Drops, Brush Brush)
        {
            _Drops = Drops;
            _Steps = Steps;
            _Brush = Brush;
        }

        public void Filter(FloatingImage Image, FloatingImage Field)
        {
            for (int i = 0; i < Field.Width; ++i)
            {
                for (int j = 0; j < Field.Height; ++j)
                {
                    Field[i, j] = Image[i, j];
                }
            }
            Random Random = new Random();
            for (int i = 0; i < _Drops; ++i)
            {
                int X = Random.Next(0, Image.Width);
                int Y = Random.Next(0, Image.Height);
                HandleDrop(X, Y, Image, Field);
            }
        }

        private void HandleDrop(int X, int Y, FloatingImage Image, FloatingImage Field)
        {
            Random Random = new Random();
            int x = X;
            int y = Y;
            double Velocity = 0;
            double Sediment = 0;
            double Height = Field[X, Y].Luminosity();
            for (int i = 0; i < _Steps; ++i)
            {
                if (x < 2 || x > Image.Width || y < 2 || y > Image.Height) break;

                Pair<int, int> Lowest = _NEIGHBORS[Random.Next(0, _NEIGHBORS.Length)];
                double H = Field[Lowest.First + x, Lowest.Second + y].Luminosity();
                foreach (Pair<int, int> N in _NEIGHBORS)
                {
                    double h = Field[N.First + x, N.Second + y].Luminosity();
                    if (h < H)
                    {
                        Lowest = N;
                        H = h;
                    }
                }

                double Slope = Height - H;
                Velocity += Slope;
                double MaxSediment = Math.Max(.1, Slope) * Velocity * (1 - ((float)i / (_Steps - 1)));
                double Delta = 0;
                if (Slope < 0)
                {
                    Velocity = 0;
                    Delta = Slope;
                }
                else Delta = .5 * (MaxSediment - Sediment);
                if (Delta > Slope) Delta = Slope;
                Sediment += Delta;
                _Brush.Color = new FloatingColor(-(float)Delta, -(float)Delta, -(float)Delta);
                _Brush.Paint(x, y, Field);
                x += Lowest.First;
                y += Lowest.Second;
                Height = H;
            }
        }
    }
}
