using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.Noise
{
    public class ElasticNoiseMap : NoiseMap
    {
        int _Grain;

        public ElasticNoiseMap(Random Random, float Width, float Height, int Octaves, int Grain, float Persistence)
            : base(Random, (int)Math.Ceiling(Width / Grain), (int)Math.Ceiling(Height / Grain), Octaves, Persistence)
        {
            _Grain = Grain;
        }

        public ElasticNoiseMap(Random[][] Random, float Width, float Height, int Octaves, int Grain, float Persistence)
            : base(Random, (int)Math.Ceiling(Width / Grain), (int)Math.Ceiling(Height / Grain), Octaves, Persistence)
        {
            _Grain = Grain;
        }

        public override float this[int x, int y]
        {
            get { return _Field[(int)(y / _Grain)][(int)(x / _Grain)]; }
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
