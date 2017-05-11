using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

using Cardamom.Graphing;

using ROTNS.Model;

namespace ROTNS.View
{
    public class RegionView
    {
        Region _Region;
        WorldView _Parent;
        Action<RegionView> _View;
        bool _Highlighted;

        public Region Region { get { return _Region; } }
        public WorldView Parent { get { return _Parent; } }

        public RegionView(WorldView Parent, Region Region)
        {
            _Parent = Parent;
            _Region = Region;
        }

        public void SetView(Action<RegionView> View)
        { 
            _View = View;
            View(this);
            if (_Highlighted) Highlight();
        }

        public void SetHighlight(bool Highlighted)
        {
            if (Highlighted != _Highlighted)
            {
                _View(this);
                if (Highlighted) Highlight();
            }
            _Highlighted = Highlighted;
        }

        private void Highlight()
        {
            foreach (MicroRegion R in Region.Regions)
            {
                if(!R.Oceanic) _Parent.MultiplyColor(R.X, R.Y, new Color(255, 80, 80));
            }
        }

        public void SetColor(Color Color, bool Border = true, bool Oceanic = false)
        {
            foreach (MicroRegion Region in _Region.Regions)
            {
                if ((!Region.Oceanic || Oceanic) && (!Region.Border || !Border)) _Parent.SetColor(Region.X, Region.Y, Color);
                else if (Region.Border && Border) _Parent.SetColor(Region.X, Region.Y, Color.Black);
            }
        }

        public void ChangeViewByPoint(Func<MicroRegion, Color> View, bool Border = true, bool Oceanic = false)
        {
            foreach (MicroRegion R in Region.Regions)
            {
                if ((!R.Oceanic || Oceanic) && (!R.Border || !Border)) _Parent.SetColor(R.X, R.Y, View(R));
                else if (R.Border && Border) _Parent.SetColor(R.X, R.Y, Color.Black);
            }          
        }

        public void ChangeViewByPoint(Func<MicroRegion, float> View, Color Color1, Color Color2, bool Border = true, bool Oceanic = false)
        {
            foreach (MicroRegion R in Region.Regions)
            {
                if ((!R.Oceanic || Oceanic) && (!R.Border || !Border)) _Parent.SetColor(R.X, R.Y, Cardamom.Utilities.ColorMath.BlendColors(Color1, Color2, View(R)));
                else if (R.Border && Border) _Parent.SetColor(R.X, R.Y, Color.Black);
            }
        }
    }
}
