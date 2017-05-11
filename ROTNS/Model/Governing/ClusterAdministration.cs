using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROTNS.Model.Governing
{
    class ClusterAdministration : Administration
    {
        List<Administration> _Subjects;

        public override IEnumerable<Region> Regions
        {
            get
            {
                foreach (Administration Subject in _Subjects)
                {
                    foreach (Region Region in Subject.Regions) yield return Region;
                }
            }
        }
        public override IEnumerable<Administration> Administrations
        {
            get
            {
                yield return this;
                foreach (Administration Subject in _Subjects)
                {
                    foreach (Administration Administration in Subject.Administrations) yield return Administration;
                }
            }
        }

        public ClusterAdministration(Administration Capitol)
        {
            _Subjects = new List<Administration>() { Capitol };
        }

		public override bool CanLegislate(Administration Administration)
		{
			if (Administration == this) return true;
			if (GovernmentForm.RegionControl && Administrations.Contains(Administration)) return true;
			if (GovernmentForm.SubRegionControl && Administration.Parents.Contains(this)) return true;
			return false;
		}

        public override void AddLaw(Law Law)
		{
			_Subjects.ForEach(i => i.AddLaw(Law));
        }

		public override void Update()
		{
			_Subjects.ForEach(i => i.Update());
		}
    }
}
