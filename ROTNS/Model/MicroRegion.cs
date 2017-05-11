using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Graphing;

using SFML.Graphics;
using ROTNS.Model;
using ROTNS.View;

namespace ROTNS.Model
{
	public class MicroRegion : Pathable<MicroRegion>
    {
        int _X;
        int _Y;
        World _Parent;
        Biome _Biome;
        Region _Region;
        bool _Oceanic;
        bool _Coastal;
        bool _Border;

        public int X { get { return _X; } }
        public int Y { get { return _Y; } }
        public Biome Biome { get { return _Biome; } }
        public bool Oceanic { get { return _Oceanic; } }
        public bool Coastal { get { return _Coastal; } set { _Coastal = value; } }
        public bool Border { get { return _Border; } set { _Border = value; } }
        public float Moisture { get { return _Parent.Moisture(_X, _Y); } }
        public float Temperature { get { return _Parent.TemperatureAt(_X, _Y); } }
        public float Height { get { return _Parent.HeightAt(_X, _Y); } }
        public Region Region { get { return _Region; } set { _Region = value; } }

        public bool Passable { get { return true; } }
		public double DistanceTo(MicroRegion Pathable)
        {
            MicroRegion R = (MicroRegion)Pathable;
            double Base = _Biome.RegionSlow + Math.Abs(R.Height - Height) * 10;
            if (R._X != _X && R._Y != _Y) Base += .5;
            if (_Oceanic) return Base + 10;
            else if (_Biome == R._Biome) return Base;
            else return Base + 5;
        }
		public double HeuristicDistanceTo(MicroRegion Pathable)
		{
			return DistanceTo(Pathable);
		}
		public IEnumerable<MicroRegion> Neighbors() { return _Parent.Neighbors(_X, _Y); }

        public MicroRegion(int X, int Y, Biome Biome, World World, bool Oceanic)
        {
            _X = X;
            _Y = Y;
            _Parent = World;
            _Biome = Biome;
            _Oceanic = Oceanic;
        }

        public override string ToString()
        {
            return "{" + _X + "," + _Y + "}, " + _Biome + ", " + _Oceanic;
        }


    }
}
