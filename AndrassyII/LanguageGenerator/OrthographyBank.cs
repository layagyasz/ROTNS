using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;
using Cardamom.Utilities.StableMatching;
using Cardamom.Utilities;

namespace AndrassyII.LanguageGenerator
{
	class OrthographyBank
	{
		private enum Attribute { ORTHOGRAPHY, MODIFIERS };

		List<SymbolSet> _Orthography;
		List<Matched> _Modifiers;

		public List<SymbolSet> Orthography { get { return _Orthography; } }

		public OrthographyBank(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			_Orthography = (List<SymbolSet>)attributes[(int)Attribute.ORTHOGRAPHY];
			_Modifiers = (List<Matched>)attributes[(int)Attribute.MODIFIERS];
		}

		public List<Matched> PickSymbols(Random Random)
		{
			List<Matched> M = new List<Matched>();
			foreach (SymbolSet S in _Orthography)
			{
				if (Random.NextDouble() < S.Frequency) M.AddRange(S.Symbols);
			}
			return M;
		}

		public List<Pair<Matched, Matched>> CreateStableMatching(List<Matched> Sounds, List<Matched> Symbols)
		{
			StableMatching<Matched, Matched> StableMatching = new StableMatching<Matched, Matched>();
			foreach (Matched Sound in Sounds)
			{
				StableMatching.AddPrimaryActor(Sound);
			}
			foreach (Matched Symbol in Symbols)
			{
				StableMatching.AddSecondaryActor(Symbol);
				foreach (Matched Sound in Sounds)
				{
					StableMatching.SetPrimaryPreference(Sound, Symbol, Sound.GetPreference(Symbol));
					StableMatching.SetSecondaryPreference(Symbol, Sound, Sound.Frequency);
				}
			}
			return StableMatching.GetPairs();
		}
	}
}
