using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ROTNS.Model.Governing;

using Venetia;

namespace ROTNS.Model
{
    public abstract class Agent : Ticked
    {
        Administration _Administration;

		protected bool _Remove;
		public bool Remove { get { return _Remove; } }

        public Administration Administration { get { return _Administration; } set { _Administration = value; } }

        public abstract bool ProposeTrade(Agent Agent, Trade Trade);
        public abstract void Tick(Random Random);
    }
}
