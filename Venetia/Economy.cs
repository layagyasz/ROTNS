using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Cardamom.Serialization;

namespace Venetia
{
	public class Economy
	{
		private enum Attribute { PROCESSES, TANGIBLES, RESOURCES };

		Service _Labor;

		Dictionary<string, Process> _Processes;
		Dictionary<string, Tangible> _Tangibles;
		Dictionary<string, Resource> _Resources;

		public IEnumerable<Process> Processes
		{
			get
			{
				foreach (KeyValuePair<string, Process> P in _Processes)
					yield return P.Value;
			}
		}
		public IEnumerable<Good> Goods
		{
			get
			{
				return _Tangibles.Where(i => i.Value is Good).Select(i => (Good)i.Value);
			}
		}
		public IEnumerable<Service> Services
		{
			get
			{
				return _Tangibles.Where(i => i.Value is Good).Select(i => (Service)i.Value);
			}
		}
		public IEnumerable<Resource> Resources
		{
			get
			{
				return _Tangibles.Where(i => i.Value is Good).Select(i => (Resource)i.Value);
			}
		}
		public IEnumerable<Tangible> All
		{
			get
			{
				return _Tangibles.Select(i => i.Value);
			}
		}
		public Service Labor
		{
			get
			{
				return _Labor;
			}
		}

		public Tangible this[string Name] { get { return _Tangibles[Name]; } }

		public Economy(
			ParseBlock Block,
			Func<ParseBlock, Good> GoodParser = null,
			Func<ParseBlock, Service> ServiceParser = null,
			Func<ParseBlock, Resource> ResourceParser = null,
			Func<ParseBlock, Process> ProcessParser = null)
		{
			Block.AddParser<Tangible>("tangible", i => new Tangible(i));
			Block.AddParser<Good>("good", GoodParser != null ? GoodParser : i => new Good(i));
			Block.AddParser<Service>("service", ServiceParser != null ? ServiceParser : i => new Service(i));
			Block.AddParser<Resource>("resource", ResourceParser != null ? ResourceParser : i => new Resource(i));
			Block.AddParser<Process>("process", ProcessParser != null ? ProcessParser : i => new Process(i));

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			_Processes = ((Dictionary<string, Process>)attributes[(int)Attribute.PROCESSES]);
			_Tangibles = ((Dictionary<string, Tangible>)attributes[(int)Attribute.TANGIBLES]);
			_Resources = ((Dictionary<string, Resource>)attributes[(int)Attribute.RESOURCES]);
			_Labor = (Service)_Tangibles["labor"];
		}

		public Economy(
			string Path,
			Func<ParseBlock, Good> GoodParser = null,
			Func<ParseBlock, Service> ServiceParser = null,
			Func<ParseBlock, Resource> ResourceParser = null,
			Func<ParseBlock, Process> ProcessParser = null)
			: this(new ParseBlock(File.ReadAllText(Path)), GoodParser, ServiceParser, ResourceParser, ProcessParser)
		{ }

		public void AddProcess(string Name, Process Process)
		{
			_Processes.Add(Name.ToLower(), Process);
		}
		public void AddTangible(string Name, Tangible Tangible)
		{
			_Tangibles.Add(Name.ToLower(), Tangible);
		}
	}
}
