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
        List<SymbolSet> _Orthography = new List<SymbolSet>();
        List<Matched> _Modifiers = new List<Matched>();

        public List<SymbolSet> Orthography { get { return _Orthography; } }

        public OrthographyBank(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                switch (B.Name.Trim().ToLower())
                {
                    case "symbols": ReadSymbols(B); break;
                    case "modifiers": ReadModifiers(B); break;
                }
            }
        }

        private void ReadSymbols(ParseBlock Block)
        {
            foreach(ParseBlock B in Block.Break())
            {
                _Orthography.Add(new SymbolSet(B));
            }
        }

        private void ReadModifiers(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                _Modifiers.Add(new Matched(new Sound(B)));
            }
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
