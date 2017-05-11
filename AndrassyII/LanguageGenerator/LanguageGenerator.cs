using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;
using Cardamom.Utilities.StableMatching;
using Cardamom.Serialization;

namespace AndrassyII.LanguageGenerator
{
    public class LanguageGenerator
    {
        OrthographyBank _OrthographyBank;
        SoundPicker _SoundPicker;
        List<Sound> _Sounds = new List<Sound>();

        public LanguageGenerator(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                switch (B.Name.Trim().ToLower())
                {
                    case "orthography": _OrthographyBank = new OrthographyBank(B); break;
                    case "sounds":
                        foreach (ParseBlock b in B.Break()) _Sounds.Add(new Sound(b));
                        break;
                    case "soundlocks": _SoundPicker = new SoundPicker(B); break;
                }
            }
        }

        public string TempGen(Random Random)
        {
            List<Sound> PickedSounds = _SoundPicker.ChooseSounds(_Sounds, Random);
            List<Matched> M = new List<Matched>();
            foreach (Sound S in PickedSounds) M.Add(new Matched(S));
            List<Matched> Symbols = _OrthographyBank.PickSymbols(Random);
            while(Symbols.Count < M.Count) Symbols = _OrthographyBank.PickSymbols(Random);
            List<Pair<Matched, Matched>> SM = _OrthographyBank.CreateStableMatching(M, Symbols);
            string R = "";
            foreach (Pair<Matched, Matched> P in SM) R += String.Format("{0} : {1}", P.First, P.Second) + "\r\n";
            return R + "\r\n";
        }
    }
}
