﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SFML.Graphics;

using AndrassyII;
using Cardamom.Serialization;
using Cence;
using Venetia;
using ROTNS.Model.Flags;

namespace ROTNS.Model
{
    public class MapGeneratorSettings
    {
        int _Width;
        int _Height;
        int _Regions;
        Color[] _NationColors;
        BiomeMap _BiomeMap;
        FlagColorMap _FlagColorMap;
        Language _Language;
        Economy _Economy;
        FlagData _FlagData;

        LatticeNoiseSettings _Terrain = new LatticeNoiseSettings();
        LatticeNoiseSettings _Moisture = new LatticeNoiseSettings();
        LatticeNoiseSettings _Population = new LatticeNoiseSettings();
        LatticeNoiseSettings _Resource = new LatticeNoiseSettings();
        LatticeNoiseSettings _Culture = new LatticeNoiseSettings();

        float _MaxLatitude;
        float _MinLatitude;
        float _WaterLevel;

        NaturalResource[] _Resources;

        public int Width { get { return _Width; } set { _Width = value; } }
        public int Height { get { return _Height; } set { _Height = value; } }
        public int Regions { get { return _Regions; } set { _Regions = value; } }
        public Color[] NationColors { get { return _NationColors; } set { _NationColors = value; } }
        public BiomeMap BiomeMap { get { return _BiomeMap; } set { _BiomeMap = value; } }
        public FlagColorMap FlagColorMap { get { return _FlagColorMap; } set { _FlagColorMap = value; } }
        public Language Language { get { return _Language; } set { _Language = value; } }
        public Economy Economy { get { return _Economy; } set { _Economy = value; } }
        public FlagData FlagData { get { return _FlagData; } set { _FlagData = value; } }

        public LatticeNoiseSettings Terrain { get { return _Terrain; } }
        public LatticeNoiseSettings Moisture { get { return _Moisture; } }
        public LatticeNoiseSettings Population { get { return _Population; } }
        public LatticeNoiseSettings Resource { get { return _Resource; } }
        public LatticeNoiseSettings Culture { get { return _Culture; } }

        public float MaxLatitude { get { return _MaxLatitude; } set { _MaxLatitude = value; } }
        public float MinLatitude { get { return _MinLatitude; } set { _MinLatitude = value; } }
        public float WaterLevel { get { return _WaterLevel; } set { _WaterLevel = value; } }

        public NaturalResource[] Resources { get { return _Resources; } set { _Resources = value; } }
    }
}
