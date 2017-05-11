using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace ROTNS.Model.Flags
{
    public class FlagData
    {
        WeightedVector<FlagTemplate> _Templates = new WeightedVector<FlagTemplate>();
        TextureSheet _Textures = new TextureSheet(1024);

        public FlagData(string Path) :
            this(ParseBlock.FromFile(Path)) { }

        public FlagData(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                string[] data = B.Name.Split('*');
                double Frequency = Convert.ToDouble(data[0], System.Globalization.CultureInfo.InvariantCulture);
                string Name = data[1].Trim().ToLower();
                _Templates.Add(Frequency, new FlagTemplate(B, _Textures));
            }
        }

        public Flag CreateFlag(Culture Culture, FlagColorMap Colors, Random Random)
        {
            FlagTemplate T = _Templates[Random.NextDouble()];
            return new Flag(T, T.SelectColors(Culture, Colors, Random));
        }
    }
}
