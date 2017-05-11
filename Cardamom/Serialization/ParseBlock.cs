using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Cardamom.Utilities;

namespace Cardamom.Serialization
{
    public class ParseBlock
    {
        Dictionary<string, Func<ParseBlock, object>> _Parsers = Cardamom.Serialization.Parse.DefaultParsers;
        Dictionary<string, object> _Scope;

        bool _ParsersOverriden;

        private string _Name;
        private string _String;
        private int _End;

        public string Name { get { return _Name; } }
        public string String { get { return _String; } }

        static readonly char[] Delimiter = { '{', '}' };

        public ParseBlock(string Source)
        {
            _End = Source.Length;
            _String = Source;
        }

        public void AddParser<T>(string TypeName, Func<ParseBlock, object> Parser, bool AddCollections = true)
        {
            if (!_ParsersOverriden) _Parsers = new Dictionary<string, Func<ParseBlock, object>>(_Parsers);
            _ParsersOverriden = true;
            _Parsers.Add(TypeName, Parser);
            if (AddCollections)
            {
                _Parsers.Add(TypeName + "[]", i => i.BreakToList<T>());
                _Parsers.Add(TypeName + "<>", i => i.BreakToDictionary<object>(true));
            }
        }

        public static ParseBlock FromFile(string Path) { return new ParseBlock(File.ReadAllText(Path)); }

        private ParseBlock(string Source, int i, char[] Delimiters)
        {
            int Start = Source.IndexOf(Delimiters[0], i);
            _Name = Source.Substring(i, Start - i).Trim();
            if (Delimiters[0] == Delimiters[1])
            {
                _End = Source.IndexOf(Delimiters[1], Start + 1);
            }
            else
            {
                int depth = 1;
                for (_End = Start + 1; _End < Source.Length; _End++)
                {
                    if (Source[_End] == Delimiters[0]) depth++;
                    else if (Source[_End] == Delimiters[1]) depth--;
                    if (depth == 0) break;
                }
            }
            Start++;
            _String = Source.Substring(Start, _End - Start).Trim();
        }

        public ParseBlock(string Source, int Start) : this(Source, Start, Delimiter) { }

        public T Parse<T>()
        {
            string[] def = Name.Split(':');
            string type = def[0].Trim().ToLower();
            string name = string.Join(":", def.Skip(1));
            _Name = name;
            if (type[0] == '!')
            {
                type = type.Substring(1);
                string[] Path = _String.Trim().Split('.');
                object Current = _Scope;
				foreach (string Node in Path) Current = ((Dictionary<string, object>)Current)[Node.ToLower()];
                return (T)Current;
            }
            else
            {
                T v = (T)_Parsers[type](this);
                return v;
            }
        }

        public Dictionary<string, T> BreakToDictionary<T>(bool Scope=false)
        {
            Dictionary<string, T> R = new Dictionary<string, T>();
            foreach (ParseBlock Block in Break())
            {
                string[] def = Block.Name.Split(':');
                string name = def[1].Trim().ToLower();
                if (_Scope != null) Block._Scope = new Dictionary<string, object>(_Scope);
                T v = Block.Parse<T>();
                if (Scope && def[0] != "!" && name != "_")
                {
                    if (_Scope == null) _Scope = new Dictionary<string, object>();
                    _Scope.Add(name, v);
                }
                R.Add(name, v);
            }
            return R;
        }

        public List<T> BreakToList<T>()
        {
            List<T> R = new List<T>();
            foreach (ParseBlock Block in Break())
            {
                if (_Scope != null) Block._Scope = new Dictionary<string, object>(_Scope);
                R.Add(Block.Parse<T>());
            }
            return R;
        }

        public List<ParseBlock> Break()
        {
            return Break(Delimiter);
        }

        public List<ParseBlock> Break(char[] Delimiters)
        {
            List<ParseBlock> Blocks = new List<ParseBlock>();
            int i = 0;
            while (i < _String.Length && _String.IndexOf(Delimiters[0], i) > -1)
            {
                ParseBlock newBlock = new ParseBlock(_String, i, Delimiters);
                newBlock._Parsers = _Parsers;
                Blocks.Add(newBlock);
                i = newBlock._End + 1;
            }
            return Blocks;
        }
    }
}
