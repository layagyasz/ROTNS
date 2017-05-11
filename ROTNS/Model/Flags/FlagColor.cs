using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace ROTNS.Model.Flags
{
    public class FlagColor
    {
        string _Name;
        Culture _Culture;
        Color _Color;
        float _Strength;
        float _Distance;

        public string Name { get { return _Name; } }
        public Culture Culture { get { return _Culture; } }
        public Color Color { get { return _Color; } }
        public float Strength { get { return _Strength; } }

        public FlagColor(string Name, Culture Culture, Color Color, float Strength)
        {
            _Name = Name;
            _Culture = Culture;
            _Color = Color;
            _Strength = Strength;

            double Total = 0;
            for (int i = 0; i < 64; ++i)
            {
                int I = (i & 32) >> 5;
                int Ind = (i & 16) >> 4;
                int L = (i & 8) >> 3;
                int P = (i & 4) >> 2;
                int T = (i & 2) >> 1;
                int U = (i & 1);

                Culture C = new Culture(I, Ind, L, P, T, U);
                Total += C.DistanceTo(Culture);
            }
            _Distance = (float)(Total / 64);
        }

        public float DistanceTo(Culture Culture)
        {
            return (float)_Culture.DistanceTo(Culture) / (_Strength * _Distance);
        }

        public float DistanceTo(FlagColor Color)
        {
            float R = (float)(_Color.R - Color.Color.R) / 255;
            float G = (float)(_Color.G - Color.Color.G) / 255;
            float B = (float)(_Color.B - Color.Color.B) / 255;
            return (float)Math.Sqrt(R * R + G * G + B * B);
        }

        public override string ToString()
        {
            return "[FlagColor]Name=" + _Name;
        }
    }
}
