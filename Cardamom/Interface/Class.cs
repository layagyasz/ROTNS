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
        public enum Mode { NA, None, Hover, Focus, Disabled, Selected, SelectedHover, SelectedDisabled };

        private Class _Hover;
        private Class _Focus;
        private Class _Disabled;
        private Class _Selected;
        private Class _SelectedHover;
        private Class _SelectedDisabled;
        private List<string> _DefinedAttributes;
        private List<string> _IncrementedAttributes;
        private Dictionary<string, object> _Attributes;

        public Class this[Mode Mode]
        {
            get
            {
                switch (Mode)
                {
                    case Mode.Hover: return _Hover;
                    case Mode.Focus: return _Focus;
                    case Mode.Disabled: return _Disabled;
                    case Mode.Selected: return _Selected;
                    case Mode.SelectedHover: return _SelectedHover;
                    case Mode.SelectedDisabled: return _SelectedDisabled;
                    default: return this;
                }
            }
        }

        public object this [string Name]
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

        public IEnumerator<string> IncrementedAttributes { get { return _IncrementedAttributes.GetEnumerator(); } }

        public Class()
        {
            _Attributes = new Dictionary<string, object>();
            _IncrementedAttributes = new List<string>();
            _DefinedAttributes = new List<string>();
        }

        public Class(Class Copy)
            : this()
        {
            if (Copy != null)
            {
                _Hover = Copy._Hover;
                _Focus = Copy._Focus;
                _Disabled = Copy._Disabled;
                _Selected = Copy._Selected;
                _SelectedHover = Copy._SelectedHover;
                _SelectedDisabled = Copy._SelectedDisabled;

                foreach (KeyValuePair<string, object> P in Copy._Attributes) _Attributes.Add(P.Key, P.Value);
                foreach (string I in Copy._IncrementedAttributes) _IncrementedAttributes.Add(I);
            }
        }

        public Class(Class Parent, ParseBlock Block)
            :this(Parent)
        {
            _Hover = this;
            _Focus = this;
            _Selected = this;
            _SelectedDisabled = this;
            _SelectedHover = this;
            _Disabled = this;

            foreach (ParseBlock B in Block.Break())
            {
                string[] def = B.Name.Split(':');
                string type = def[0].ToLower().Trim();
                string name = def[1].ToLower().Trim();
                if (type[0] == '+')
                {
                    _IncrementedAttributes.Add(name);
                    type = type.Substring(1);
                }
                _DefinedAttributes.Add(name);
                Console.WriteLine("{0} {1}", type, name);
                switch (type)
                {
                    case "int": this[name] = Convert.ToInt32(B.String); break;
                    case "int[]": this[name] = Parse.Array(B.String, Convert.ToInt32); break;
                    case "short": this[name] = Convert.ToInt16(B.String); break;
                    case "short[]": this[name] = Parse.Array(B.String, Convert.ToInt16); break;
                    case "uint": this[name] = Convert.ToInt32(B.String); break;
                    case "uint[]": this[name] = Parse.Array(B.String, Convert.ToInt32); break;
                    case "ushort": this[name] = Convert.ToUInt16(B.String); break;
                    case "ushort[]": this[name] = Parse.Array(B.String, Convert.ToUInt16); break;
                    case "byte": this[name] = Convert.ToByte(B.String); break;
                    case "byte[]": this[name] = Parse.Array(B.String, Convert.ToByte); break;
                    case "float": this[name] = Convert.ToSingle(B.String, System.Globalization.CultureInfo.InvariantCulture); break;
                    case "float[]": this[name] = Parse.Array(B.String, System.Globalization.CultureInfo.InvariantCulture, Convert.ToSingle); break;
                    case "double": this[name] = Convert.ToDouble(B.String, System.Globalization.CultureInfo.InvariantCulture); break;
                    case "double[]": this[name] = Parse.Array(B.String, System.Globalization.CultureInfo.InvariantCulture, Convert.ToDouble); break;
                    case "color": this[name] = ClassLibrary.Instance.ParseColor(B.String); break;
                    case "color[]": this[name] = ClassLibrary.Instance.ParseColors(B.String); break;
                    case "vector2f": this[name] = ClassLibrary.Instance.ParseVector2f(B.String); break;
                    case "vector2f[]": this[name] = ClassLibrary.Instance.ParseVector2fs(B.String); break;
                    case "font": this[name] = ClassLibrary.Instance.ParseFont(B.String); break;
                    case "sound": this[name] = ClassLibrary.Instance.ParseSound(B.String); break;
                    case "hover": this._Hover = new Class(this, new ParseBlock(B.String)); break;
                    case "focus": this._Focus = new Class(this, new ParseBlock(B.String)); break;
                    case "selected": this._Selected = new Class(this, new ParseBlock(B.String)); break;
                    case "disabled": this._Disabled = new Class(this, new ParseBlock(B.String)); break;
                    case "selectedhover": this._SelectedHover = new Class(this, new ParseBlock(B.String)); break;
                    case "selecteddisabled": this._SelectedDisabled = new Class(this, new ParseBlock(B.String)); break;
                }
            }
        }

        public Class(ParseBlock Block)
            : this((Block.Name.Split(':').Length > 1) ? ClassLibrary.Instance[Block.Name.Split(':')[0]] : null, Block)
        {
        }

        public Class Combine(Class Parent)
        {
            Class newClass = new Class(Parent);
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
    }
}
