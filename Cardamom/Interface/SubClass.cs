using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;
using SFML.Audio;

using Cardamom.Serialization;

namespace Cardamom.Interface
{
	public class SubClass
	{
		private List<string> _DefinedAttributes;
		private Dictionary<string, object> _Attributes;

		public object this[string Name]
		{
			get
			{
				if (_Attributes.ContainsKey(Name)) return _Attributes[Name];
				else return null;
			}
			set
			{
				if (_Attributes.ContainsKey(Name)) _Attributes[Name] = value;
				else _Attributes.Add(Name, value);
			}
		}

		public SubClass()
		{
			_Attributes = new Dictionary<string, object>();
			_DefinedAttributes = new List<string>();
		}

		public SubClass(SubClass Copy)
			: this()
		{
			Inherit(Copy);
		}

		public SubClass(ParseBlock Block)
			: this()
		{
			Dictionary<string, object> D = Block.BreakToDictionary<object>();
			foreach (var KV in D)
			{
				_DefinedAttributes.Add(KV.Key);
				switch (KV.Key)
				{
					default: Add(KV.Key, KV.Value); break;
				}
			}
		}

		public void Add(string Name, object Value)
		{
			if (_Attributes.ContainsKey(Name)) _Attributes[Name] = Value;
			else _Attributes.Add(Name, Value);
		}

		private void Inherit(SubClass Parent)
		{
			if (Parent != null)
			{
				foreach (KeyValuePair<string, object> P in Parent._Attributes) _Attributes.Add(P.Key, P.Value);
			}
		}

		public SubClass Combine(SubClass Parent)
		{
			SubClass newClass = new SubClass(Parent);
			foreach (string Attribute in _DefinedAttributes) newClass[Attribute] = this[Attribute];
			return newClass;
		}

		public object GetAttributeWithDefault(string Name, object Default)
		{
			return GetAttributeWithDefault(_Attributes, Name, Default);
		}

		public object GetAttributeWithDefault(string Name, string Parent, object Default)
		{
			return GetAttributeWithDefault(_Attributes, Name, Parent, Default);
		}

		public T GetAttributeWithDefault<T>(Dictionary<string, T> Attributes, string Name, T Default)
		{
			if (Attributes.ContainsKey(Name)) return Attributes[Name];
			else return Default;
		}

		public T GetAttributeWithDefault<T>(Dictionary<string, T> Attributes, string Name, string Parent, T Default)
		{
			if (Attributes.ContainsKey(Name)) return Attributes[Name];
			else return GetAttributeWithDefault(Attributes, Parent, Default);
		}

		public T GetInterpolatedAttribute<T>(string Name, T Default, Class Class, Func<T, T, T> Interpolator)
		{
			T From = (T)GetAttributeWithDefault(Name, Default);
			T To = (T)GetAttributeWithDefault(Name, Default);
			return Interpolator(From, To);
		}
	}
}
