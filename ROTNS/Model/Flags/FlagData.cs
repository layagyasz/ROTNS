using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Graphing;
using Cardamom.Serialization;
using Cardamom.Utilities;

using SFML.Window;

namespace ROTNS.Model.Flags
{
	public class FlagData
	{
		WeightedVector<FlagTemplate> _Templates = new WeightedVector<FlagTemplate>();

		public FlagData(string Path) :
			this(ParseBlock.FromFile(Path))
		{ }

		public FlagData(ParseBlock Block)
		{
			Block.AddParser<FlagTemplate>("flag-template", i => new FlagTemplate(i));
			Block.AddParser<GraphNode<Vector2f[]>>("node",
				i => new GraphNode<Vector2f[]>(Cardamom.Interface.ClassLibrary.Instance.ParseVector2fs(i.String)));

			foreach (FlagTemplate template in Block.BreakToArray<FlagTemplate>())
				_Templates.Add(template.Frequency, template);
		}

		public Flag CreateFlag(Culture Culture, FlagColorMap Colors, Random Random)
		{
			FlagTemplate T = _Templates[Random.NextDouble()];
			return new Flag(T, T.SelectColors(Culture, Colors, Random));
		}
	}
}
