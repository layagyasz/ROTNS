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
		Dictionary<string, object> _Scope = new Dictionary<string, object>();

		bool _ParsersOverriden;

		private string _Name;
		private string _String;
		private int _End;

		public string Name { get { return _Name; } }
		public string String { get { return _String; } }

		static readonly char[] DELIMITERS = { '{', '}' };

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
				_Parsers.Add(TypeName + "<>", i => i.BreakToDictionary<T>(true));
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

		public ParseBlock(string Source, int Start) : this(Source, Start, DELIMITERS) { }

		public KeyValuePair<string, T> Parse<T>(Dictionary<string, object> Scope)
		{
			if (Scope != null) _Scope = new Dictionary<string, object>(Scope);

			string[] def = Name.ToLower().Split(':');
			string type = def[0].Trim();
			string name = string.Join(":", def.Skip(1));
			_Name = name;
			if (type[0] == '!')
			{
				type = type.Substring(1);
				string[] Path = _String.Trim().Split('.');
				object Current = _Scope;
				foreach (string Node in Path) Current = ((dynamic)Current)[Node.ToLower()];
				return new KeyValuePair<string, T>(name, (T)Current);
			}
			else
			{
				T v = (T)_Parsers[type](this);
				return new KeyValuePair<string, T>(name, v);
			}
		}

		private IEnumerable<KeyValuePair<string, T>> ParseAll<T>(Dictionary<string, object> Scope)
		{
			foreach (ParseBlock Block in Break()) yield return Block.Parse<T>(Scope);
		}

		private Func<KeyValuePair<string, T>, KeyValuePair<string, T>> KeepScope<T>(bool Scope)
		{
			return i => { if (Scope && _Scope != null && i.Key != "_") _Scope.Add(i.Key, i.Value); return i; };
		}

		public IEnumerable<KeyValuePair<string, T>> BreakToEnumerable<T>(bool Scope = false)
		{
			return ParseAll<T>(_Scope).Select(KeepScope<T>(Scope));
		}

		public Dictionary<string, T> BreakToDictionary<T>(bool Scope = false)
		{
			return BreakToEnumerable<T>(Scope).ToDictionary(i => i.Key, i => i.Value);
		}

		private static string FixAttributeName(string Name)
		{
			return Name.Replace('-', '_').ToUpper();
		}

		public T[] BreakToAttributes<T>(Type EnumType, bool Scope = false)
		{
			IEnumerable<KeyValuePair<string, T>> attributes = ParseAll<T>(_Scope).Select(KeepScope<T>(Scope));
			T[] attributeArray = new T[Enum.GetValues(EnumType).Cast<int>().Max() + 1];
			foreach (var P in attributes) attributeArray[(int)Enum.Parse(EnumType, FixAttributeName(P.Key))] = P.Value;
			return attributeArray;
		}

		public List<T> BreakToList<T>()
		{
			return ParseAll<T>(_Scope).Select(i => i.Value).ToList();
		}

		public T[] BreakToArray<T>()
		{
			return ParseAll<T>(_Scope).Select(i => i.Value).ToArray();
		}

		public Tuple<T1, T2> BreakTo2Tuple<T1, T2>()
		{
			List<object> values = BreakToList<object>();
			return Tuple.Create((T1)values[0], (T2)values[1]);
		}

		public Tuple<T1, T2, T3> BreakTo3Tuple<T1, T2, T3>()
		{
			List<object> values = BreakToList<object>();
			return Tuple.Create((T1)values[0], (T2)values[1], (T3)values[2]);
		}

		private IEnumerable<ParseBlock> Break()
		{
			return Break(DELIMITERS);
		}

		private IEnumerable<ParseBlock> Break(char[] Delimiters)
		{
			int i = 0;
			while (i < _String.Length && _String.IndexOf(Delimiters[0], i) > -1)
			{
				ParseBlock newBlock = new ParseBlock(_String, i, Delimiters);
				newBlock._Parsers = _Parsers;
				yield return newBlock;
				i = newBlock._End + 1;
			}
		}
	}
}
