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

		string _Type;
		string _Name;
		string _String;
		IEnumerable<ParseBlock> _Blocks;

		public string Type { get { return _Type; } }
		public string Name { get { return _Name; } }
		public string String { get { return _String; } }

		static readonly char[] DELIMITERS = { '{', '}' };

		public ParseBlock(string Source)
		{
			_String = Source;
		}

		public ParseBlock(IEnumerable<ParseBlock> Blocks)
		{
			_Blocks = Blocks;
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

		public ParseBlock(string Type, string Name, string Source)
		{
			_Type = Type;
			_Name = Name;
			_String = Source;
		}

		public ParseBlock(string Type, string Name, IEnumerable<ParseBlock> Blocks)
		{
			_Type = Type;
			_Name = Name;
			_Blocks = Blocks;
		}

		public KeyValuePair<string, T> Parse<T>(Dictionary<string, object> Scope)
		{
			if (_Name == null) return Break().First().Parse<T>(Scope);

			if (Scope != null) _Scope = new Dictionary<string, object>(Scope);

			if (_Type[0] == '!')
			{
				string type = _Type.Substring(1);
				string[] Path = _String.Trim().Split('.');
				object Current = _Scope;
				try
				{
					foreach (string Node in Path) Current = ((dynamic)Current)[Node.ToLower()];
					return new KeyValuePair<string, T>(_Name, (T)Current);
				}
				catch (KeyNotFoundException exception)
				{
					throw new Exception(
						string.Format("Could not find object '{0}'\n{1}", string.Join(".", Path), exception.Message));
				}
			}
			else
			{
				try
				{
					T v = (T)_Parsers[_Type](this);
					return new KeyValuePair<string, T>(_Name, v);
				}
				catch (KeyNotFoundException exception)
				{
					throw new Exception(
						string.Format("No parser for '{0}'\n{1}", string.Join(".", _Type), exception.Message));
				}
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

		public IEnumerable<ParseBlock> Break()
		{
			return Break(DELIMITERS);
		}

		public IEnumerable<ParseBlock> Break(char[] Delimiters)
		{
			if (_Blocks == null)
			{
				int i = 0;
				while (i < _String.Length && _String.IndexOf(Delimiters[0], i) > -1)
				{
					int start = _String.IndexOf(Delimiters[0], i);
					int end;

					string name = _String.Substring(i, start - i).Trim();
					string[] def = name.ToLower().Split(':');
					string type = def[0].Trim();
					name = string.Join(":", def.Skip(1));

					int depth = 1;
					for (end = start + 1; end < _String.Length; end++)
					{
						if (_String[end] == Delimiters[0]) depth++;
						else if (_String[end] == Delimiters[1]) depth--;
						if (depth == 0) break;
					}
					start++;
					string blockString = _String.Substring(start, end - start).Trim();
					yield return new ParseBlock(type, name, blockString) { _Parsers = this._Parsers };
					i = end + 1;
				}
			}
			else
			{
				foreach (ParseBlock block in _Blocks)
				{
					block._Parsers = this._Parsers;
					yield return block;
				}
			}
		}
	}
}
