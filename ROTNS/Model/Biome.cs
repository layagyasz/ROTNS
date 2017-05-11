using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace ROTNS.Model
{
    public class Biome
    {
        float _Height;
        float _Temperature;
        float _Moisture;
        float _RegionSlow;
        Color _Color;
        Image _Image;

        public Color Color { get { return _Color; } }
        public float RegionSlow { get { return _RegionSlow; } }

        public Biome(float Height, float Temperature, float Moisture, float RegionSlow, Color Color, Image Image)
        {
            _Height = Height;
            _Temperature = Temperature;
            _Moisture = Moisture;
            _RegionSlow = RegionSlow;
            _Color = Color;
            _Image = Image;
        }

        public Color ColorAt(int X, int Y)
        {
            return Cardamom.Utilities.ColorMath.Multiply(_Color, _Image.GetPixel((uint)(X % _Image.Size.X), (uint)(Y % _Image.Size.Y)));
        }

        public float Distance(float Height, float Temperature, float Moisture)
        {
            return (float)Math.Sqrt(Math.Pow(Height - _Height, 2) + Math.Pow(Temperature - _Temperature, 2) + Math.Pow(Moisture - _Moisture, 2));
        }
    }
}
