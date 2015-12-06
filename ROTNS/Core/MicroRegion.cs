using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Graphing;

using SFML.Graphics;
using ROTNS.Core;
using ROTNS.OverMapping;

namespace ROTNS.Core
{
    public class MicroRegion : Pathable
    {
        int _X;
        int _Y;
        World _Parent;
        Biome _Biome;
        Region _Region;
        bool _Oceanic;
        bool _Coastal;

        public int X { get { return _X; } }
        public int Y { get { return _Y; } }
        public Biome Biome { get { return _Biome; } }
        public bool Oceanic { get { return _Oceanic; } }
        public bool Coastal { get { return _Coastal; } set { _Coastal = value; } }
        public float Moisture { get { return _Parent.Moisture(_X, _Y); } }
        public float Temperature { get { return _Parent.Temperature(_X, _Y); } }
        public float Height { get { return _Parent.Height(_X, _Y); } }
        public Region Region { get { return _Region; } set { _Region = value; } }
        public Color Color { get { return _Parent.GetColor(_X, _Y); } set { _Parent.SetColor(_X, _Y,value); } }

        public bool Passable { get { return true; } }
        public double DistanceTo(Pathable Pathable)
        {
            MicroRegion R = (MicroRegion)Pathable;
            double Base = _Biome.RegionSlow + Math.Abs(R.Height - Height) * 20;
            if (R._X != _X && R._Y != _Y) Base += .5;
            if (_Oceanic) return Base + 20;
            else if (_Biome == R._Biome) return Base;
            else return Base + 10;
        }
        public IEnumerator<Pathable> Neighbors() { return _Parent.Neighbors(_X, _Y); }

        public MicroRegion(int X, int Y, Biome Biome, World World, bool Oceanic)
        {
            _X = X;
            _Y = Y;
            _Parent = World;
            _Biome = Biome;
            _Oceanic = Oceanic;
        }

        public void AddPopulation(float Population)
        {
            _Region.AddPopulation(Population);
        }

        public override string ToString()
        {
            return "{" + _X + "," + _Y + "}, " + _Biome + ", " + _Oceanic;
        }


    }
}
