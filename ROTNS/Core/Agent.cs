using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Venetia;

namespace ROTNS.Core
{
    public abstract class Agent
    {
        Administration _Administration;

        public Administration Administration { get { return _Administration; } set { _Administration = value; } }

        public abstract bool ProposeTrade(Agent Agent, Trade Trade);
        public abstract void Tick();
    }
}
