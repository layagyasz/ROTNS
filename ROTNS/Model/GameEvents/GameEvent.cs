using System;
using System.Collections.Generic;

namespace ROTNS.Model.GameEvents
{
	public abstract class GameEvent : EventArgs
	{
		public abstract string Name { get; }
		public abstract string Description { get; }
		public abstract IEnumerable<KeyValuePair<string, Action<Region>>> BranchTriggers { get; }

		public abstract void Apply(Region Region);
	}
}
