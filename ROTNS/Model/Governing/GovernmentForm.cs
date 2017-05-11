using System;
using System.Linq;
using System.Collections.Generic;

namespace ROTNS
{
	public class GovernmentForm
	{
		private bool _Voluntary;
		private bool _Integrated;
		private bool _Devolved;
		private bool _RegionControl;
		private bool _SubRegionControl;

		private bool _UnanimousConsent;

		private bool _Tributary;

		public bool Voluntary { get { return _Voluntary; } }
		public bool Integrated { get { return _Integrated; } }
		public bool Devolved { get { return _Devolved; } }
		public bool RegionControl { get { return _RegionControl; } }
		public bool SubRegionControl { get { return _SubRegionControl; } }
		public bool UnanimousConsent { get { return _UnanimousConsent; } }
		public bool Tributary { get { return _Tributary; } }

		public bool IsValid()
		{
			return (!SubRegionControl || RegionControl) &&
				(Voluntary || !UnanimousConsent) &&
				(Devolved || SubRegionControl);
		}

		public static IEnumerable<GovernmentForm> AllValidGovernmentForms(Func<GovernmentForm, bool> Filter)
		{
			return Enumerable.Range(0, 128)
				.Select(i => Convert.ToString(i, 2).PadLeft(7, '0').Select(j => j == '1').ToArray())
				.Select(i => new GovernmentForm()
				{
					_Voluntary = i[0],
				  	_Integrated = i[1],
					_Devolved = i[2],
				  	_RegionControl = i[3],
				 	_SubRegionControl = i[4],
					_UnanimousConsent = i[5],
					_Tributary = i[6]
				})
				.Where(i => i.IsValid() && Filter(i));
		}

		public override string ToString()
		{
			return string.Format("[GovernmentForm: Voluntary={0}, Integrated={1}, Devolved={2}, RegionControl={3}, SubRegionControl={4}, UnanimousConsent={5}, Tributary={6}]", Voluntary, Integrated, Devolved, RegionControl, SubRegionControl, UnanimousConsent, Tributary);
		}
	}
}
