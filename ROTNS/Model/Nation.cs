using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Graphing;

using SFML.Graphics;

namespace ROTNS.Model
{
    public class Nation : DijkstraRegion<Region>
    {
        Region _Capitol;
        List<Region> _Regions = new List<Region>();
        Color _Color;

        public Region Center { get { return _Capitol; } }
        public double StartDistance { get { return 0; } }
        public Color Color { get { return _Color; } }

        public Nation(Region Capitol, Color Color) { _Capitol = Capitol; _Color = Color; Console.WriteLine(Color); }

        public void Add(Region Region) { _Regions.Add(Region); Region.Nation = this; }
    }
}
