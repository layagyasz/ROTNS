using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.Noise
{
    public class SmoothNoiseMap : NoiseMap
    {
        int _Grain;

        protected SmoothNoiseMap() { }

        public SmoothNoiseMap(Random Random, float Width, float Height, int Octaves, int Grain, float Persistence)
            : base(Random, (int)Math.Ceiling(Width / Grain), (int)Math.Ceiling(Height / Grain), Octaves, Persistence)
        {
            _Grain = Grain;
        }

        public SmoothNoiseMap(Random[][] Random, float Width, float Height, int Octaves, int Grain, float Persistence)
            : base(Random, (int)Math.Ceiling(Width / Grain), (int)Math.Ceiling(Height / Grain), Octaves, Persistence)
        {
            _Grain = Grain;
        }

        public override NoiseMap ApplyFilter(Filter Filter)
        {
            return new SmoothNoiseMap() { _Field = this.Filter(Filter), _Grain = this._Grain };
        }

        public override float this[int x, int y]
        {
            get
            {
                int nx = (int)(x / _Grain);
                int ny = (int)(y / _Grain);
                int ux = (nx + 1 < _Field[0].Length) ? nx + 1 : nx;
                int uy = (ny + 1 < _Field.Length) ? ny + 1 : ny;
                float cy = (float)(y - ny * _Grain) / _Grain;
                float cx = (float)(x - nx * _Grain) / _Grain;

                return (_Field[uy][ux] * cy + _Field[ny][ux] * (1 - cy)) * cx
                    + (_Field[uy][nx] * cy + _Field[ny][nx] * (1 - cy)) * (1 - cx);
            }
        }

        public override void Multiply(NoiseMap NoiseMap)
        {
            for (int i = 0; i < _Field.Length; ++i)
            {
                for (int j = 0; j < _Field[i / _Grain].Length; ++j)
                {
                    _Field[i][j] *= NoiseMap[j * _Grain, i * _Grain];
                }
            }
        }

        public override void Add(NoiseMap NoiseMap, float A)
        {
            for (int i = 0; i < _Field.Length; ++i)
            {
                for (int j = 0; j < _Field[i].Length; ++j)
                {
                    _Field[i][j] = _Field[i][j] * A + (1 - A) * NoiseMap[j * _Grain, i * _Grain];
                }
            }
        }
    }
}
