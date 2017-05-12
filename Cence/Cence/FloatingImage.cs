using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace Cence
{
    public class FloatingImage
    {
        FloatingColor[,] _Pixels;

        public int Height { get { return _Pixels.GetLength(1); } }
        public int Width { get { return _Pixels.GetLength(0); } }

        public FloatingColor this[int X, int Y]
        {
            get
            {
                if (X >= _Pixels.GetLength(0)) X = _Pixels.GetLength(0) - 1;
                if (Y >= _Pixels.GetLength(1)) Y = _Pixels.GetLength(1) - 1;
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;

                return _Pixels[X, Y];
            }
            set
            {
                if (X >= 0 && X < _Pixels.GetLength(0) && Y >= 0 && Y < _Pixels.GetLength(1)) _Pixels[X, Y] = value;
            }
        }

        private FloatingImage() { }

        public FloatingImage(int Width, int Height) { _Pixels = new FloatingColor[Width, Height]; }

        public FloatingImage(Image Image)
        {
            _Pixels = new FloatingColor[Image.Size.X, Image.Size.Y];

            for (uint i = 0; i < Image.Size.Y; ++i)
            {
                for(uint j=0; j<Image.Size.X; ++j)
                {
                    _Pixels[j, i] = new FloatingColor(Image.GetPixel(j, i));
                }
            }
        }

        public FloatingImage(string Image)
            : this(new Image(Image)) { }

		public FloatingImage(float[,] Map, Channel Channel)
		{
			_Pixels = new FloatingColor[Map.GetLength(0), Map.GetLength(1)];
			for (int i = 0; i < Width; ++i)
			{
				for (int j = 0; j < Height; ++j)
				{
					_Pixels[i, j] = new FloatingColor(Map[i, j], Channel);
				}
			}
		}

		public float[,] GetChannel(Channel Channel)
		{
			float[,] C = new float[Width, Height];
			for (int i = 0; i<Width; ++i)
			{
				for (int j = 0; j<Height; ++j)
				{
					C[i, j] = _Pixels[i, j].GetChannel(Channel);
				}
			}
			return C;
		}

        public IEnumerable<Pixel> GetPixels()
        {
            return GetPixels(0, 0, Width, Height);
        }

        public IEnumerable<Pixel> GetPixels(int MinX, int MinY, int MaxX, int MaxY)
        {
            for (int j = Math.Max(MinY, 0); j < Math.Min(MaxY, Height) ; ++j)
                for (int i = Math.Max(MinX, 0); i < Math.Min(MaxX, Width); ++i)
                    yield return new Pixel(i, j, _Pixels[i, j]);
        }

        public FloatingImage Filter(Filter Filter)
        {
            int Width = _Pixels.GetLength(0);
            int Height = _Pixels.GetLength(1);

            FloatingColor[,] NewPixels = new FloatingColor[Width, Height];

            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    NewPixels[i, j] = Filter.Filter(i, j, this) * Filter.Factor + Filter.Bias;
                }
            }

            return new FloatingImage() { _Pixels = NewPixels };
        }

        public FloatingImage Filter(BlendFilter Filter, FloatingImage Image)
        {
            int Width = _Pixels.GetLength(0);
            int Height = _Pixels.GetLength(1);

            FloatingColor[,] NewPixels = new FloatingColor[Width, Height];

            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    NewPixels[i, j] = Filter.Filter(this[i, j], Image[i, j]);
                }
            }

            return new FloatingImage() { _Pixels = NewPixels };
        }

        public FloatingImage Filter(TranscendentalFilter Filter)
        {
            int Width = _Pixels.GetLength(0);
            int Height = _Pixels.GetLength(1);

            FloatingImage R = new FloatingImage(Width, Height);

            Filter.Filter(this, R);

            return R;
        }

        public FloatingImage(FloatingImage[,] SplitImages)
        {
            int eWidth = SplitImages[0, 0]._Pixels.GetLength(0);
            int eHeight = SplitImages[0, 0]._Pixels.GetLength(1);

            _Pixels = new FloatingColor[SplitImages.GetLength(0) * eWidth, SplitImages.GetLength(1) * eHeight];

            for (int i = 0; i < SplitImages.GetLength(0); ++i)
            {
                for (int j = 0; j < SplitImages.GetLength(1); ++j)
                {
                    for (int x = 0; x < eWidth; ++x)
                    {
                        for (int y = 0; y < eHeight; ++y)
                        {
                            _Pixels[i * eWidth + x, j * eHeight + y] = SplitImages[i, j][x, y];
                        }
                    }
                }
            }
        }

        public Image ConvertToImage()
        {
            uint Width = (uint)_Pixels.GetLength(0);
            uint Height = (uint)_Pixels.GetLength(1);

            Image I = new Image(Width, Height);

            for (uint i = 0; i < Height; ++i)
            {
                for (uint j = 0; j < Width; ++j)
                {
                    I.SetPixel(j, i, _Pixels[j, i].ConvertToColor());
                }
            }
            return I;
        }

        public FloatingImage[,] Split(int Width, int Height)
        {
            uint ImageWidth = (uint)_Pixels.GetLength(0);
            uint ImageHeight = (uint)_Pixels.GetLength(1);

            int xSplit = (int)(ImageWidth / Width);
            int ySplit = (int)(ImageHeight / Height);

            FloatingImage[,] F = new FloatingImage[xSplit, ySplit];

            for (int i = 0; i < ImageHeight; ++i)
            {
                for (int j = 0; j < ImageWidth; ++j)
                {
                    int fX = j / Width;
                    int fY = i / Height;

                    int fOX = j - Width * fX;
                    int fOY = i - Height * fY;

                    if (F[fX, fY] == null) F[fX, fY] = new FloatingImage(Width, Height);
                    F[fX, fY]._Pixels[fOX, fOY] = this[j, i];
                }
            }

            return F;
        }
    }
}
