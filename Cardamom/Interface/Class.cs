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
	public class Class
	{
		public enum Mode { None, Hover, Focus, Disabled, Selected, SelectedHover, SelectedDisabled };

		string _Name;
		private SubClass[] _SubClasses = new SubClass[7];
		private List<string> _IncrementedAttributes = new List<string>();

		public string Name { get { return _Name; } }
		public SubClass this[Mode Mode] { get { return _SubClasses[(int)Mode]; } }
		public IEnumerable<string> IncrementedAttributes { get { return _IncrementedAttributes; } }

		public static implicit operator Class(string ClassName)
		{
			return ClassLibrary.Instance[ClassName];
		}

		public Class() { }

		public Class(Class Copy)
			: this()
		{
			Inherit(Copy);
		}

		public Class(ParseBlock Block)
			: this()
		{
			_Name = Block.Name;

			Dictionary<string, object> D = Block.BreakToDictionary<object>();
			if (D.ContainsKey("parent"))
			{
				Inherit((Class)D["parent"]);
				D.Remove("parent");
			}
			if (D.ContainsKey("default"))
			{
				SubClass newDefault = ((SubClass)D["default"]).Combine(_SubClasses[(int)Mode.None]);
				for (int i = 0; i < _SubClasses.Length; ++i) _SubClasses[i] = newDefault;
				D.Remove("default");
			}
			if (D.ContainsKey("fade"))
			{
				_IncrementedAttributes = (List<string>)D["fade"];
				D.Remove("fade");
			}
			foreach (var KV in D)
			{
				SubClass newClass = ((SubClass)KV.Value).Combine(_SubClasses[(int)Mode.None]);
				switch (KV.Key)
				{
					case "hover": _SubClasses[(int)Mode.Hover] = newClass; break;
					case "focus": _SubClasses[(int)Mode.Focus] = newClass; break;
					case "selected": _SubClasses[(int)Mode.Selected] = newClass; break;
					case "disabled": _SubClasses[(int)Mode.Disabled] = newClass; break;
					case "selected-hover": _SubClasses[(int)Mode.SelectedHover] = newClass; break;
					case "selected-disabled": _SubClasses[(int)Mode.SelectedDisabled] = newClass; break;
				}
			}
		}

		public void Add(string Name, object Value, bool Increment = false)
		{
			Array.ForEach(_SubClasses, i => i.Add(Name, Value));
			if (Increment && !_IncrementedAttributes.Contains(Name)) _IncrementedAttributes.Add(Name);
		}

		public void Add(Mode Mode, string Name, object Value, bool Increment = false)
		{
			_SubClasses[(int)Mode].Add(Name, Value);
			if (Increment && !_IncrementedAttributes.Contains(Name)) _IncrementedAttributes.Add(Name);
		}

		private void Inherit(Class Parent)
		{
			if (Parent != null)
			{
				_SubClasses = Parent._SubClasses.ToArray();
				_IncrementedAttributes.AddRange(Parent._IncrementedAttributes);
			}
		}

		public Class Combine(Class Parent)
		{
			return new Class() { _SubClasses = _SubClasses.Zip(Parent._SubClasses, (i, j) => j.Combine(i)).ToArray() };
		}

		public object GetAttributeWithDefault(string Name, object Default)
		{
			return this[Mode.None].GetAttributeWithDefault(Name, Default);
		}

		public object GetAttributeWithDefault(string Name, string Parent, object Default)
		{
			return this[Mode.None].GetAttributeWithDefault(Name, Parent, Default);
		}
	}
}
