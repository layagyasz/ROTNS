using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Venetia;

namespace ROTNS.Core
{
    public class RegionAdministration : Administration
    {
        Region _Region;

        public Region Region { get { return _Region; } }

        public RegionAdministration(Region Region) { _Region = Region; }
    }
}