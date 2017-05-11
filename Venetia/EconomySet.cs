using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Venetia
{
    public class EconomySet<T> : IEnumerable<KeyValuePair<string, T>>
    {
        Dictionary<string, T> _Items = new Dictionary<string, T>();

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator() { return _Items.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[string Name]
        {
            get { return _Items[Name.ToLower()]; }
        }

        public void Load<K>(ParseBlock Block, EconomySet<K> References, Func<ParseBlock, EconomySet<K>,T> Constructor)
        {
            foreach (ParseBlock B in Block.Break())
            {
                _Items.Add(B.Name.ToLower(), Constructor.Invoke(B, References));
            }
        }

        public void Load(ParseBlock Block, Func<ParseBlock, T> Constructor)
        {
            foreach (ParseBlock B in Block.Break())
            {
                _Items.Add(B.Name.ToLower(), Constructor.Invoke(B));
            }
        }

        public void Add(string Key, T Item) { _Items.Add(Key.ToLower(), Item); }

        public void Add(KeyValuePair<string, T> Pair) { _Items.Add(Pair.Key.ToLower(), Pair.Value); }
    }
}
