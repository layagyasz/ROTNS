using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Venetia;

namespace ROTNS.Model.Governing
{
    public class RegionAdministration : Administration
    {
        Region _Region;

        public Region Region { get { return _Region; } }
        public override IEnumerable<Region> Regions { get { yield return _Region; } }
        public override IEnumerable<Administration> Administrations { get { yield return this; } }

        public RegionAdministration(Region Region) { _Region = Region; Name = Region.Name; }

		public override bool CanLegislate(Administration Administration)
		{
			return this == Administration;
		}

        public override void AddLaw(Law Law)
        {
            _LegalCode.AddLaw(Law);
			Law.Apply(_Region);
        }

		public override void Update()
		{
			_Region.Update();
			_LegalCode.Update(_Region);
		}
    }
}