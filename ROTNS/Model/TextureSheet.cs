using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace ROTNS.Model
{
    public class TextureSheet
    {
        Image _Image;
        int _X;
        int _Y;
        int _nY;

        Dictionary<string, IntRect> _Contained = new Dictionary<string, IntRect>();

        public TextureSheet(uint Size)
        {
            _Image = new Image(Size, Size);
        }

        public bool ImageExists(string Key) { return _Contained.ContainsKey(Key); }

        public IntRect AddImage(string Key, Image Image)
        {
            if (_Contained.ContainsKey(Key)) return _Contained[Key];
            else
            {
                if (_X + Image.Size.X > _Image.Size.X)
                {
                    _X = 0;
                    _Y = _nY;
                }

                IntRect R = new IntRect(_X, _Y, (int)Image.Size.X, (int)Image.Size.Y);
                CopyImageTo(Image, _Image, (uint)_X, (uint)_Y);

                if (_Y + Image.Size.Y > _nY) _nY = _Y + (int)Image.Size.Y;
                _X += (int)Image.Size.X;

                _Contained.Add(Key, R);

                return R;
            }
        }

        private void CopyImageTo(Image Image, Image CopyTo, uint X, uint Y)
        {
            for (uint i = 0, cI = X; i < Image.Size.X; ++i, ++cI)
            {
                for (uint j = 0, cJ = Y; j < Image.Size.Y; ++j, ++cJ)
                {
                    CopyTo.SetPixel(cI, cJ, Image.GetPixel(i, j));
                }
            }
        }

        public Texture Compile() { return new Texture(_Image); }

        public void SaveToFile(string File) { _Image.SaveToFile(File); }
    }
}
