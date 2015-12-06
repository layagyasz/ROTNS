using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SFML.Graphics;

using AndrassyII;
using Cardamom.Serialization;
using Venetia;

namespace ROTNS.Core
{
    public class MapGeneratorSettings
    {
        int _Width;
        int _Height;
        int _Regions;
        Color[] _NationColors;
        BiomeMap _BiomeMap;
        Language _Language;
        Economy _Economy;

        NoiseSettings _Terrain = new NoiseSettings();
        NoiseSettings _Moisture = new NoiseSettings();
        NoiseSettings _Population = new NoiseSettings();
        NoiseSettings _Resource = new NoiseSettings();
        NoiseSettings _Culture = new NoiseSettings();

        float _MaxLatitude;
        float _MinLatitude;
        float _WaterLevel;

        NaturalResource[] _Resources;

        public int Width { get { return _Width; } set { _Width = value; } }
        public int Height { get { return _Height; } set { _Height = value; } }
        public int Regions { get { return _Regions; } set { _Regions = value; } }
        public Color[] NationColors { get { return _NationColors; } set { _NationColors = value; } }
        public BiomeMap BiomeMap { get { return _BiomeMap; } set { _BiomeMap = value; } }
        public Language Language { get { return _Language; } set { _Language = value; } }
        public Economy Economy { get { return _Economy; } set { _Economy = value; } }

        public NoiseSettings Terrain { get { return _Terrain; } }
        public NoiseSettings Moisture { get { return _Moisture; } }
        public NoiseSettings Population { get { return _Population; } }
        public NoiseSettings Resource { get { return _Resource; } }
        public NoiseSettings Culture { get { return _Culture; } }

        public float MaxLatitude { get { return _MaxLatitude; } set { _MaxLatitude = value; } }
        public float MinLatitude { get { return _MinLatitude; } set { _MinLatitude = value; } }
        public float WaterLevel { get { return _WaterLevel; } set { _WaterLevel = value; } }

        public NaturalResource[] Resources { get { return _Resources; } set { _Resources = value; } }
    }
}
