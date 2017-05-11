using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace Cardamom.Utilities
{
    public class ColorMath
    {

        public static Color Multiply(Color c1, Color c2)
        {
            float R = ((float)c1.R / 255) * ((float)c2.R / 255);
            float G = ((float)c1.G / 255) * ((float)c2.G / 255);
            float B = ((float)c1.B / 255) * ((float)c2.B / 255);
            float A = ((float)c1.A / 255) * ((float)c2.A / 255);
            return new Color((byte)(R * 255), (byte)(G * 255), (byte)(B * 255), (byte)(A * 255));
        }

        private static double HueToRGB(double P, double Q, double T)
        {
            if (T < 0) T += 1;
            if (T > 1) T -= 1;
            if (T < .16666666667) return P + (Q - P) * 6 * T;
            if (T < .5) return Q;
            if (T < .666666667) return P + (Q - P) * (.6666666667 - T) * 6;
            return P;
        }

        public static Color HSLToRGB(double H, double S, double L)
        {
            double r, g, b;

            if (S == 0)
            {
                r = g = b = L;
            }
            else
            {
                double q = L < 0.5 ? L * (1 + S) : L + S - L * S;
                double p = 2 * L - q;
                r = HueToRGB(p, q, H + .3333333);
                g = HueToRGB(p, q, H);
                b = HueToRGB(p, q, H - .3333333);
            }
            return new Color((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        public static Color BlendColors(Color Color1, Color Color2, float a)
        {
            if (float.IsNaN(a) || float.IsInfinity(a)) return new Color(100, 100, 100);

            byte r = (byte)(Color1.R * (1 - a) + Color2.R * a);
            byte g = (byte)(Color1.G * (1 - a) + Color2.G * a);
            byte b = (byte)(Color1.B * (1 - a) + Color2.B * a);
            byte A = (byte)(Color1.A * (1 - a) + Color2.A * a);

            return new Color(r, g, b, A);
        }

        public static Color MakeColor(float R, float G, float B, float A = 1f)
        {
            return new Color((byte)(R * 255), (byte)(G * 255), (byte)(B * 255), (byte)(A * 255));
        }
    }
}
