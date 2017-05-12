using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class FlowField : TranscendentalFilter
    {
        int _Actors;
        float _Alpha;
        float _Angle;
        float _Velocity;
        int _Steps;
        Gradient _Gradient;

        public FlowField(Gradient Gradient, float Angle, float Alpha, float Velocity, int Steps, int Actors)
        {
            _Gradient = Gradient;
            _Actors = Actors;
            _Alpha = Alpha;
            _Angle = Angle;
            _Velocity = Velocity;
            _Steps = Steps;
        }

        public void Filter(FloatingImage Image, FloatingImage Field)
        {
            for (int i = 0; i < Field.Width; ++i)
            {
                for (int j = 0; j < Field.Height; ++j)
                {
                    Field[i, j] = new FloatingColor(0, 0, 0, 1f);
                }
            }
            Random Random = new Random();
            for (int i = 0; i < _Actors; ++i)
            {
                int X = Random.Next(0, Image.Width);
                int Y = Random.Next(0, Image.Height);
                HandleActor(X, Y, Image, Field);
            }
        }

        private void HandleActor(int X, int Y, FloatingImage Image, FloatingImage Field)
        {
            double Angle = _Angle;
            double x = X;
            double y = Y;
            for (int i = 0; i < _Steps; ++i)
            {
                if (x < 0 || x > Image.Width || y < 0 || y > Image.Height) break;

                Field[(int)x, (int)y] += _Gradient.Evaluate((float)i / (_Steps - 1));
                double NewAngle = Image[(int)x, (int)y].Luminosity() * 2 * Math.PI;
                Angle = _Alpha * NewAngle + (1 - _Alpha) * Angle;
                x += _Velocity * Math.Cos(Angle);
                y += _Velocity * Math.Sin(Angle);
            }
        }
    }
}
