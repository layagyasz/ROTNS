using System;

using ROTNS.Model;
using ROTNS.Model.Governing;

namespace ROTNS
{
	public abstract class Law
	{
		Administration _Issuer;

		public Law()
		{
		}

		public abstract void Apply(Region Region);
		public abstract void Remove(Region Region);
		public abstract void Update(Region Region);
	}
}
