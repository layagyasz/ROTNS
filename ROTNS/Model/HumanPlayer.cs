using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Venetia;

namespace ROTNS.Model
{
    public class HumanPlayer : Agent
    {
        public override bool ProposeTrade(Agent Agent, Trade Trade)
        {
            Console.WriteLine("################################");
            Console.WriteLine("---------Trade Proposal---------");
            Console.WriteLine("################################");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0} -> {1}", ((Region)Trade.Zone1).Name, ((Region)Trade.Zone2).Name);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0} {1}", Trade.Amount1, Trade.Good1.Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("For");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("{0} {1}", Trade.Amount2, Trade.Good2.Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Commercial stats");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Z1 {0}", Trade.Flow2);
            Console.WriteLine("Z2 {0}", Trade.Flow1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Profit {0}", Trade.Profit);
            Console.ForegroundColor = ConsoleColor.Gray;

            return Convert.ToBoolean(Console.ReadLine());
        }

        public override void Tick(Random Random) { }
    }
}
