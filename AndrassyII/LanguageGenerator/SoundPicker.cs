using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace AndrassyII.LanguageGenerator
{
    class SoundPicker
    {
        List<SoundClass> _SoundClasses = new List<SoundClass>();

        public SoundPicker(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                _SoundClasses.Add(new SoundClass(B));
            }
        }

        public List<Sound> ChooseSounds(List<Sound> Available, Random Random)
        {
            List<Sound> C = new List<Sound>();
			IEnumerable<SoundClass> Classes = _SoundClasses.Where(i => Random.NextDouble() < i.Frequency);
            foreach (Sound Sound in Available)
            {
                foreach (SoundClass Class in Classes)
                {
                    if(Sound.MatchesClass(Class) && 
                        Random.NextDouble() > Sound.Drop) 
                    {
                        C.Add(Sound);
                        break;
                    }
                    else if (Random.NextDouble() < Sound.Frequency)
                    {
                        C.Add(Sound);
                        break;
                    }
                }
            }
            return C;
        }
    }
}
