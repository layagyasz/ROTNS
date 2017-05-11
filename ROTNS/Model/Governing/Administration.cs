using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ROTNS.Model.Flags;

namespace ROTNS.Model.Governing
{
	public abstract class Administration
	{
		string _Name;
		Agent _Agent;
		GovernmentForm _GovernmentForm;
		Administration _Parent;
		OrganizationClass _AdministrationType;
		Flag _Flag;

		protected LegalCode _LegalCode;

		public string Name { get { return _Name; } set { _Name = value; } }
		public Agent Agent { get { return _Agent; } set { _Agent = value; } }
		public GovernmentForm GovernmentForm { get { return _GovernmentForm; } set { _GovernmentForm = value; } }
        public Administration Parent { get { return _Parent; } set { _Parent = value; } }
        public OrganizationClass AdministrationType { get { return _AdministrationType; } set { _AdministrationType = value; } }
        public Flag Flag { get { return _Flag; } set { _Flag = value; } }

        public abstract IEnumerable<Region> Regions { get; }
        public abstract IEnumerable<Administration> Administrations { get; }
        public IEnumerable<Administration> Parents
        {
            get
            {
                Administration Current = this;
                while (Current != null)
                {
                    yield return Current;
                    Current = Current.Parent;
                }
            }
        }

		public abstract bool CanLegislate(Administration Administration);
        public abstract void AddLaw(Law Law);
		public abstract void Update();

        public Administration GetRoot()
        {
            return Parents.Last();
        }

    }
}
