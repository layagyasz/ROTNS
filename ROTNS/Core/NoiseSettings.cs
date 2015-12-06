using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROTNS.Core
{
    public class NoiseSettings
    {
        int _Octaves;
        float _Persistence;
        int _Grain;

        public int Octaves { get { return _Octaves; } set { _Octaves = value; } }
        public float Persistence { get { return _Persistence; } set { _Persistence = value; } }
        public int Grain { get { return _Grain; } set { _Grain = value; } }
    }
}
