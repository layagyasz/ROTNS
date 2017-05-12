using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace Cence
{
    public struct FloatingColor
	{
        private static readonly float _E = .00001f;

        float _R;
        float _G;
        float _B;
        float _A;

        public float R { get { return _R; } set { _R = value; } }
        public float G { get { return _G; } set { _G = value; } }
        public float B { get { return _B; } set { _B = value; } }
        public float A { get { return _A; } set { _A = value; } }

        public int Dimensions { get { return 4; } }
        public IComparable this[int Dimension]
        {
            get
            {
                switch(Dimension)
                {
                    case 0: return _R;
                    case 1: return _G;
                    case 2: return _B;
                    case 3: return _A;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch(Dimension)
                {
                    case 0: _R = (float)value; break;
                    case 1: _G = (float)value; break;
                    case 2: _B = (float)value; break;
                    case 3: _A = (float)value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public FloatingColor(float R, float G, float B)
        {
            _R = R;
            _G = G;
            _B = B;
            _A = 1;
        }

        public FloatingColor(float R, float G, float B, float A)
        {
            _R = R;
            _G = G;
            _B = B;
            _A = A;
        }

        public FloatingColor(FloatingColor Copy)
            : this(Copy.R, Copy.G, Copy.B, Copy.A) { }

        public FloatingColor(byte R, byte G, byte B)
            : this((float)R / 255, (float)G / 255, (float)B / 255) { }

        public FloatingColor(byte R, byte G, byte B, byte A)
            : this((float)R / 255, (float)G / 255, (float)B / 255, (float)A / 255) { }

        public FloatingColor(Color Color)
            : this(Color.R, Color.G, Color.B) { }

        public FloatingColor(float Value)
            : this(Value, Value, Value) { }

		public FloatingColor(float Value, Channel Channel)
			: this(Channel == Channel.RED ? Value : 0,
				   Channel == Channel.GREEN ? Value : 0,
				   Channel == Channel.BLUE ? Value : 0,
				   Channel == Channel.ALPHA ? Value : 0)
		{ }

		public FloatingColor(byte Value, Channel Channel)
					: this(Channel == Channel.RED ? Value : 0,
						   Channel == Channel.GREEN ? Value : 0,
						   Channel == Channel.BLUE ? Value : 0,
						   Channel == Channel.ALPHA ? Value : 0)
		{ }

        public Color ConvertToColor()
        {
            byte r = (byte)(Math.Min(Math.Max(_R, 0), 1) * 255);
            byte g = (byte)(Math.Min(Math.Max(_G, 0), 1) * 255);
            byte b = (byte)(Math.Min(Math.Max(_B, 0), 1) * 255);
            byte a = (byte)(Math.Min(Math.Max(_A, 0), 1) * 255);

            return new Color(r, g, b, a);
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

        private static FloatingColor HSLToRGB(double H, double S, double L)
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
            return new FloatingColor((float)r, (float)g, (float)b);
        }

        public FloatingColor MakeRGB()
        {
            return HSLToRGB(R, G, B);
        }

        public FloatingColor MakeHSL()
        {
            float Max = Math.Max(_R, Math.Max(_G, _B));
            float Min = Math.Min(_R, Math.Min(_G, _B));
            float L = (Max + Min) / 2;
            if (Max - Min < .00000001) return new FloatingColor(0, 0, L);
            float D = Max - Min;
            float S = (L > .5) ? D / (2 - Max - Min) : D / (Max + Min);
            float H = 0;
            if (Math.Abs(_R - Max) < .00000001) H = (_G - _B) / D + ((_G < _B) ? 6 : 0);
            else if (Math.Abs(_G - Max) < .00000001) H = 2 + (_B - _R) / D;
            else H = 4 + (_R - _G) / D;
            return new FloatingColor(H / 6, S, L);
        }

		public float GetChannel(Channel Channel)
		{
			switch (Channel)
			{
				case Channel.RED: return _R;
				case Channel.GREEN: return _G;
				case Channel.BLUE: return _B;
				case Channel.ALPHA: return _A;
				default: throw new Exception(string.Format("Invalid Channel {0}", Channel));
			}
		}

        public float Luminosity()
        {
            return (Math.Max(_R, Math.Max(_G, _B)) + Math.Min(_R, Math.Min(_G, _B))) / 2;
        }

        public static FloatingColor operator *(FloatingColor P, float S)
        {
            return new FloatingColor(P._R * S, P._G * S, P._B * S, P._A);
        }

        public static FloatingColor operator *(float S, FloatingColor P)
        {
            return new FloatingColor(P._R * S, P._G * S, P._B * S, P._A);
        }

        public static FloatingColor operator *(int S, FloatingColor P)
        {
            return P * S;
        }

        public static FloatingColor operator +(FloatingColor P1, FloatingColor P2)
        {
            return new FloatingColor(P1._R + P2._R, P1._G + P2._G, P1._B + P2._B, P1._A);
        }

        public static FloatingColor operator +(FloatingColor P, float S)
        {
            return new FloatingColor(P._R + S, P._G + S, P._B + S, P._A);
        }

        public static FloatingColor operator +(float S, FloatingColor P)
        {
            return new FloatingColor(P._R + S, P._G + S, P._B + S, P._A);
        }

        public static FloatingColor operator -(FloatingColor P1, FloatingColor P2)
        {
            return new FloatingColor(P1._R - P2._R, P1._G - P2._G, P1._B - P2._B, P1._A);
        }

        public static FloatingColor operator -(FloatingColor P, float S)
        {
            return new FloatingColor(P._R - S, P._G - S, P._B - S, P._A);
        }

        public static FloatingColor operator -(float S, FloatingColor P)
        {
            return new FloatingColor(S - P._R, S - P._G, S - P._B, P._A);
        }

        public static FloatingColor operator *(FloatingColor P1, FloatingColor P2)
        {
            return new FloatingColor(P1._R * P2._R, P1._G * P2._G, P1._B * P2._B, P1._A);
        }

        public static FloatingColor operator /(FloatingColor P1, FloatingColor P2)
        {
            return new FloatingColor(P1._R / P2._R, P1._G / P2._G, P1._B / P2._B, P1._A);
        }

        public static bool operator ==(FloatingColor P1, FloatingColor P2)
        {
            return P1.Equals(P2);
        }

        public static bool operator !=(FloatingColor P1, FloatingColor P2)
        {
            return !(P1 == P2);
        }

        public override string ToString()
        {
            return String.Format("[Color]R={0},G={1},B={2},A={3}", R, G, B, A);
        }

        public override bool Equals(object obj)
        {
            FloatingColor F = (FloatingColor)obj;
            return Math.Abs(F.R - _R) < _E &&
                Math.Abs(F.G - _G) < _E &&
                Math.Abs(F.B - _B) < _E &&
                Math.Abs(F.A - _A) < _E;
        }

        public override int GetHashCode()
        {
            int Hash = _R.GetHashCode();
            Hash = unchecked(Hash * 31 + _G.GetHashCode());
            Hash = unchecked(Hash * 31 + _B.GetHashCode());
            Hash = unchecked(Hash * 31 + _A.GetHashCode());
            return Hash;
        }
    }
}
