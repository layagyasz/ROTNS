using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

using Cardamom.Graphing;

using ROTNS.Core;

namespace ROTNS.OverMapping
{
    public class MapRegion
    {
        Region _Region;
        OverMap _Parent;
        Color _Color;
        List<MicroRegion> _Regions = new List<MicroRegion>();

        public Region Region { get { return _Region; } set { _Region = value; } }
        public OverMap Parent { get { return _Parent; } }
        public Color Color { get { return _Color; } set { _Color = value; foreach (MicroRegion R in _Regions) if (!R.Oceanic && !(R.Color.R == 0 && R.Color.G == 0 && R.Color.B == 0)) R.Color = value; else if(R.Oceanic) R.Color = R.Biome.Color; } }

        public MapRegion(OverMap Parent, Region Region)
        {
            _Parent = Parent;
            foreach (MicroRegion M in Region.Regions) _Regions.Add(M);
        }

        public void BiomeColor()
        {
            foreach (MicroRegion R in _Regions)
                if (!(R.Color.R == 0 && R.Color.G == 0 && R.Color.B == 0)) R.Color = R.Biome.Color;
        }

        public void TemperatureColor()
        {
            foreach (MicroRegion R in _Regions)
            {
                if (!R.Oceanic && !(R.Color.R == 0 && R.Color.G == 0 && R.Color.B == 0))
                {
                    float a = R.Temperature;
                    R.Color = new Color((byte)(a * 255), 0, (byte)((1 - a) * 255));
                }
            }
        }

        public void MoistureColor()
        {
            foreach (MicroRegion R in _Regions)
            {
                if (!R.Oceanic && !(R.Color.R == 0 && R.Color.G == 0 && R.Color.B == 0))
                {
                    float a = R.Moisture;
                    R.Color = new Color((byte)((1 - a) * 255), (byte)((1 - a) * 255), (byte)(a * 255));
                }
            }
        }

        public void CultureColor(byte Index)
        {
            foreach (MicroRegion R in _Regions)
            {
                float a = 0;
                Color B = Color.Black;
                switch (Index)
                {
                    case 0: a = _Region.Culture.Individualism; B = new Color(0, 255, 0); break;
                    case 1: a = _Region.Culture.Indulgence; B = new Color(255, 0, 255); break;
                    case 2: a = _Region.Culture.LongTermOrientation; B=new Color(0, 0, 255); break;
                    case 3: a = _Region.Culture.PowerDistance; B=new Color(255, 255, 0); break;
                    case 4: a = _Region.Culture.Toughness; B=new Color(255, 0, 0); break;
                    case 5: a = _Region.Culture.UncertaintyAvoidance; B=new Color(0, 255, 255); break;
                }
                Color M = Cardamom.Utilities.ColorMath.BlendColors(Color.White, B, a);
                if (!R.Oceanic && !(R.Color.R == 0 && R.Color.G == 0 && R.Color.B == 0))
                {
                    R.Color = M;
                }
            }
        }

        public float GetResourceDensity(NaturalResource Resource) { return _Region.GetResourceDensity(Resource); }

        public void DiscoverBorder()
        {
            foreach(MicroRegion Current in _Regions)
            {
                IEnumerator<Pathable> it = Current.Neighbors();
                while (it.MoveNext())
                {
                    MicroRegion N = (MicroRegion)it.Current;
                    if (N.Region != _Region && !Current.Oceanic && _Region.Area >= N.Region.Area) Current.Color = Color.Black;
                }
            }
        }
    }
}
