using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

namespace Cardamom.Utilities.Noise
{
    public class NoiseMap
    {
        protected float[][] _Field;

        protected NoiseMap() { }

        public NoiseMap(Random Random, int Width, int Height, int Octaves, float Persistence)
        {
            _Field = PerlinNoise(WhiteNoise(Width, Height, Random), Octaves, Persistence);
        }

        public NoiseMap(Random[][] Random, int Width, int Height, int Octaves, float Persistence)
        {
            _Field = PerlinNoise(WhiteNoise(Width, Height, Random), Octaves, Persistence);
        }

        public virtual NoiseMap ApplyFilter(Filter Filter)
        {
            return new NoiseMap() { _Field = this.Filter(Filter) };
        }

        public void Normalize()
        {
            float max = 0;
            float min = Single.MaxValue;
            for (int i = 0; i < _Field.Length; i++)
            {
                for (int j = 0; j < _Field[0].Length; j++)
                {
                    if (_Field[i][j] > max) max = _Field[i][j];
                    if (_Field[i][j] < min) min = _Field[i][j];
                }
            }
            float diff = max - min;
            for (int i = 0; i < _Field.Length; i++)
            {
                for (int j = 0; j < _Field[0].Length; j++)
                {
                    _Field[i][j] = (_Field[i][j] - min) / diff;
                }
            }
        }

        public virtual void Multiply(NoiseMap NoiseMap)
        {
            for (int i = 0; i < _Field.Length; ++i)
            {
                for (int j = 0; j < _Field[i].Length; ++j)
                {
                    _Field[i][j] *= NoiseMap[j, i];
                }
            }
        }

        public virtual void Add(NoiseMap NoiseMap, float A)
        {
            for (int i = 0; i < _Field.Length; ++i)
            {
                for (int j = 0; j < _Field[i].Length; ++j)
                {
                    _Field[i][j] = _Field[i][j] * A + (1 - A) * NoiseMap[j, i];
                }
            }
        }

        public virtual float this[int x, int y]
        {
            get { return _Field[y][x]; }
        }

        public float GetHeightCutoff(float Perc)
        {
            float[] All = new float[_Field.Length * _Field[0].Length];
            for (int i = 0; i < _Field.Length; i++)
            {
                for (int j = 0; j < _Field[i].Length; j++)
                {
                    All[i * _Field[0].Length + j] = _Field[i][j];
                }
            }
            Array.Sort(All);

            int index = (int)Math.Ceiling(Perc * (All.Length - 1));
            return All[index];
        }

        private float Interpolate(float x0, float x1, float a)
        {
            return x0 * (1 - a) + a * x1;
        }

        private float[][] WhiteNoise(int Width, int Height, Random Random)
        {
            float[][] Noise = new float[Height][];
            for (int i = 0; i < Height; i++)
            {
                float[] Row = new float[Width];
                for (int j = 0; j < Width; j++)
                {
                    Row[j] = (float)Random.NextDouble() % 1;
                }
                Noise[i] = Row;
            }
            return Noise;
        }

        private float[][] WhiteNoise(int Width, int Height, Random[][] Random)
        {
            float[][] Noise = new float[Height][];
            for (int i = 0; i < Height; i++)
            {
                float[] Row = new float[Width];
                for (int j = 0; j < Width; j++)
                {
                    int x = i * Random.Length / Height;
                    int y = j * Random[x].Length / Width;
                    Row[j] = (float)Random[x][y].NextDouble();
                }
                Noise[i] = Row;
            }
            return Noise;
        }

        private float[][] SmoothNoise(float[][] WN, int Octaves)
        {
            float[][] SN = new float[WN.Length][];

            int period = 1 << Octaves;
            float frequency = 1.0f / period;

            for (int i = 0; i < WN.Length; i++)
            {
                //calculate the horizontal sampling indices
                int sample_i0 = (i / period) * period;
                int sample_i1 = (sample_i0 + period) % WN.Length; //wrap around
                float horizontal_blend = (i - sample_i0) * frequency;

                float[] Row = new float[WN[i].Length];
                for (int j = 0; j < WN[i].Length; j++)
                {
                    //calculate the vertical sampling indices
                    int sample_j0 = (j / period) * period;
                    int sample_j1 = (sample_j0 + period) % WN[i].Length; //wrap around
                    float vertical_blend = (j - sample_j0) * frequency;

                    //blend the top two corners
                    float top = Interpolate(WN[sample_i0][sample_j0],
                       WN[sample_i1][sample_j0], horizontal_blend);

                    //blend the bottom two corners
                    float bottom = Interpolate(WN[sample_i0][sample_j1],
                       WN[sample_i1][sample_j1], horizontal_blend);

                    //final blend
                    Row[j] = Interpolate(top, bottom, vertical_blend);
                }
                SN[i] = Row;
            }
            return SN;
        }

        float[][] PerlinNoise(float[][] BaseNoise, int Octaves, float Persistence)
        {
            int Height = BaseNoise.Length;
            int Width = BaseNoise[0].Length;

            float[][][] smoothNoise = new float[Octaves][][]; //an array of 2D arrays containing

            //generate smooth noise
            for (int i = 0; i < Octaves; i++)
            {
                smoothNoise[i] = SmoothNoise(BaseNoise, i);
            }

            float[][] perlinNoise = new float[Height][];
            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            //blend noise together
            for (int octave = Octaves - 1; octave >= 0; octave--)
            {
                amplitude *= Persistence;
                totalAmplitude += amplitude;

                for (int i = 0; i < Height; i++)
                {
                    float[] Row = (perlinNoise[i] == null ? new float[Width] : perlinNoise[i]);
                    for (int j = 0; j < Width; j++)
                    {
                        Row[j] += smoothNoise[octave][i][j] * amplitude;
                    }
                    perlinNoise[i] = Row;
                }
            }

            //normalisation
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    perlinNoise[i][j] /= totalAmplitude;
                }
            }

            return perlinNoise;
        }

        protected float[][] Filter(Filter Filter)
        {
            float[][] R = new float[_Field.GetLength(0)][];
            int w = _Field[0].Length;
            int h = _Field.Length;


            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    if(R[j] == null) R[j] = new float[w];
                    for (int x = 0; x < Filter.Width; ++x)
                    {
                        for (int y = 0; y < Filter.Height; ++y)
                        {
                            int iX = (i - Filter.Width / 2 + x + w) % w;
                            int iY = (j - Filter.Height / 2 + y + h) % h;
                            R[j][i] += (float)(_Field[iY][iX] * Filter[x, y]);
                        }
                    }
                    R[j][i] = (float)Math.Min(Math.Max(Filter.Factor * R[j][i] + Filter.Bias, 0), 1);
                }
            }

            return R;
        }
    }
}