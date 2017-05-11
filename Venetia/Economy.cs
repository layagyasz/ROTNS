using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Venetia
{
    public class Economy
    {
        Service _Labor = new Service("labor", 1, 1, 0);
        Resource _Property = new Resource("property", 1, 1, .1);

        EconomySet<Process> _Processes = new EconomySet<Process>();
        EconomySet<Good> _Goods = new EconomySet<Good>();
        EconomySet<Service> _Services = new EconomySet<Service>();
        EconomySet<Resource> _Resources = new EconomySet<Resource>();
        EconomySet<Tangible> _All= new EconomySet<Tangible>();

        public IEnumerable<Process> Processes { get { foreach (KeyValuePair<string, Process> P in _Processes) yield return P.Value; } }
        public IEnumerable<Good> Goods { get { foreach (KeyValuePair<string, Good> P in _Goods) yield return P.Value; } }
        public IEnumerable<Service> Services { get { foreach (KeyValuePair<string, Service> P in _Services) yield return P.Value; } }
        public IEnumerable<Resource> Resources { get { foreach (KeyValuePair<string, Resource> P in _Resources) yield return P.Value; } }
        public IEnumerable<Tangible> All { get { foreach (KeyValuePair<string, Tangible> P in _All) yield return P.Value; } }
        public Service Labor { get { return _Labor; } }
        public Resource Property { get { return _Property; } }

        public Tangible this[string Name] { get { return _All[Name]; } }

        public Economy()
        {
            AddService("labor", _Labor);
            AddResource("property", _Property);
        }

        public void LoadProcesses(ParseBlock Block, Func<ParseBlock, EconomySet<Tangible>, Process> Constructor)
        {
            _Processes.Load(Block, _All, Constructor);
        }

        public void LoadGoods(ParseBlock Block, Func<ParseBlock, Good> Constructor)
        {
            EconomySet<Good> Temp = new EconomySet<Good>();
            Temp.Load(Block, Constructor);
            foreach (KeyValuePair<string, Good> G in Temp) AddGood(G.Key, G.Value);
        }

        public void LoadServices(ParseBlock Block, Func<ParseBlock, Service> Constructor)
        {
            EconomySet<Service> Temp = new EconomySet<Service>();
            Temp.Load(Block, Constructor);
            foreach (KeyValuePair<string, Service> G in Temp) AddService(G.Key, G.Value);
        }

        public void LoadResources(ParseBlock Block, Func<ParseBlock, Resource> Constructor)
        {
            EconomySet<Resource> Temp = new EconomySet<Resource>();
            Temp.Load(Block, Constructor);
            foreach (KeyValuePair<string, Resource> G in Temp) AddResource(G.Key, G.Value);
        }

        public void AddProcess(string Name, Process Process) { _Processes.Add(Name.ToLower(), Process); }
        public void AddGood(string Name, Good Good) { _Goods.Add(Name.ToLower(), Good); _All.Add(Name.ToLower(), Good); }
        public void AddService(string Name, Service Service) { _Services.Add(Name.ToLower(), Service); _All.Add(Name.ToLower(), Service); }
        public void AddResource(string Name, Resource Resource) { _Resources.Add(Name.ToLower(), Resource); _All.Add(Name.ToLower(), Resource); }
    }
}
