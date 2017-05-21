using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Serialization
{
	public class Parse
	{
		public static readonly Dictionary<string, Func<ParseBlock, object>> DefaultParsers = new Dictionary<string, Func<ParseBlock, object>>()
		{
			{ "string", i => i.String },
			{ "string[]", i => i.String.Split(' ') },
			{ "bool", i => Convert.ToBoolean(i.String) },
			{ "bool[]", i => Parse.Array(i.String, Convert.ToBoolean) },
			{ "int", i => Convert.ToInt32(i.String) },
			{ "int[]", i => Parse.Array(i.String, Convert.ToInt32) },
			{ "short", i => Convert.ToInt16(i.String) },
			{ "short[]", i => Parse.Array(i.String, Convert.ToInt16) },
			{ "uint", i => Convert.ToInt32(i.String) },
			{ "uint[]", i => Parse.Array(i.String, Convert.ToInt32) },
			{ "ushort", i => Convert.ToUInt16(i.String) },
			{ "ushort[]", i => Parse.Array(i.String, Convert.ToUInt16) },
			{ "byte", i => Convert.ToByte(i.String) },
			{ "byte[]", i => Parse.Array(i.String, Convert.ToByte) },
			{ "float", i => Convert.ToSingle(i.String, System.Globalization.CultureInfo.InvariantCulture) },
			{ "float[]", i => Parse.Array(i.String, System.Globalization.CultureInfo.InvariantCulture, Convert.ToSingle) },
			{ "double", i => Convert.ToDouble(i.String, System.Globalization.CultureInfo.InvariantCulture) },
			{ "double[]", i => Parse.Array(i.String, System.Globalization.CultureInfo.InvariantCulture, Convert.ToDouble) },
			{ "var[]", i => i.BreakToList<object>() },
			{ "var<>", i => i.BreakToDictionary<object>() },
			{ "2-tuple", i=> i.BreakTo2Tuple<object, object>() },
			{ "2-tuple[]", i=> i.BreakToList<Tuple<object, object>>() },
			{ "2-tuple<>", i=> i.BreakToDictionary<Tuple<object, object>>() },
			{ "3-tuple", i=> i.BreakTo3Tuple<object, object, object>() },
			{ "3-tuple[]", i=> i.BreakToList<Tuple<object, object, object>>() },
			{ "3-tuple<>", i=> i.BreakToDictionary<Tuple<object, object, object>>() }
		};

		public static T[] Array<T>(string Code, System.Globalization.CultureInfo CultureInfo, Func<string, System.Globalization.CultureInfo, T> Converter)
		{
			string[] def = Code.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			T[] r = new T[def.Length];
			for (int i = 0; i < def.Length; ++i)
			{
				r[i] = Converter.Invoke(def[i], CultureInfo);
			}
			return r;
		}

		public static T[] Array<T>(string Code, Func<string, T> Converter)
		{
			string[] def = Code.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			T[] r = new T[def.Length];
			for (int i = 0; i < def.Length; ++i)
			{
				r[i] = Converter.Invoke(def[i]);
			}
			return r;
		}
	}
}
