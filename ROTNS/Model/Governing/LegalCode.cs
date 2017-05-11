using System.Collections.Generic;

namespace ROTNS.Model.Governing
{
	public class LegalCode
	{
		List<Law> _Laws = new List<Law>();

		public void AddLaw(Law Law) { _Laws.Add(Law); }

		public void Update(Region Region) { _Laws.ForEach(i => i.Update(Region)); }
	}
}