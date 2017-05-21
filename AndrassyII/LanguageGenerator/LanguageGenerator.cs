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
		private enum Attribute { SOUNDS, SOUNDLOCKS, ORTHOGRAPHY };

		OrthographyBank _OrthographyBank;
		SoundPicker _SoundPicker;
		List<Sound> _Sounds = new List<Sound>();

		public LanguageGenerator(ParseBlock Block)
		{
			Block.AddParser<OrthographyBank>("orthogrphy", i => new OrthographyBank(i));
			Block.AddParser<Sound>("sound", i => new Sound(i));
			Block.AddParser<SoundPicker>("soundlocks", i => new SoundPicker(i));
			Block.AddParser<Matched>("symbol", i => new Matched(new Sound(i)));

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			_OrthographyBank = (OrthographyBank)attributes[(int)Attribute.ORTHOGRAPHY];
			_SoundPicker = (SoundPicker)attributes[(int)Attribute.SOUNDLOCKS];
			_Sounds = (List<Sound>)attributes[(int)Attribute.SOUNDS];
		}

		public string TempGen(Random Random)
		{
			List<Sound> PickedSounds = _SoundPicker.ChooseSounds(_Sounds, Random);
			List<Matched> M = new List<Matched>();
			foreach (Sound S in PickedSounds) M.Add(new Matched(S));
			List<Matched> Symbols = _OrthographyBank.PickSymbols(Random);
			while (Symbols.Count < M.Count) Symbols = _OrthographyBank.PickSymbols(Random);
			List<Pair<Matched, Matched>> SM = _OrthographyBank.CreateStableMatching(M, Symbols);
			string R = "";
			foreach (Pair<Matched, Matched> P in SM) R += String.Format("{0} : {1}", P.First, P.Second) + "\r\n";
			return R + "\r\n";
		}
	}
}
