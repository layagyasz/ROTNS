using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;

namespace Cardamom.Interface.Items.Subcomponents
{
	class TextComponent : Component
	{
		Text _Text;
		Vector2f _Position;
		string _String;
		int _Width;
		int _Height;
		int _PaddedWidth;
		int _PaddedHeight;
		uint _Index;

		public uint Index
		{
			get { return _Index; }
			set
			{
				_Index = Math.Max(0, value);
				if (_Text.CharacterSize > 0)
				{
					int l = (int)(_PaddedHeight / _Text.CharacterSize);
					if (_Index + l > _Lines.Count) _Index = (uint)Math.Max(0, _Lines.Count - l);
					CreateStringFromLines(_Index, _PaddedHeight);
				}
			}
		}

		List<string> _Lines = new List<string>();

		public Vector2f Position { get { return _Position; } set { _Position = value; } }
		public Vector2f Size { get { return new Vector2f(_PaddedWidth, _PaddedHeight); } }

		public string DisplayedString { get { return _String; } set { _String = value; SetFormatedString(_String, _PaddedWidth, _PaddedHeight, _Index); } }

		public TextComponent(Class Class)
		{
			_Text = new Text();
			_Text.Font = Class.GetAttributeWithDefault<Font>("font-face", null);
			_Text.Color = Class.GetAttributeWithDefault<Color>("font-color", ClassLibrary.NullColor);
			_Text.CharacterSize = (uint)(int)Class.GetAttributeWithDefault("font-size", 0);
			int[] P = (int[])Class.GetAttributeWithDefault("padding", ClassLibrary.NullArray);
			_Position = new Vector2f(P[0], P[1]);
			_Width = (int)Class.GetAttributeWithDefault("width", 0);
			_Height = (int)Class.GetAttributeWithDefault("height", 0);
			DisplayedString = "";
		}

		private void SetFormatedString(string Text, int Width, int Height, uint Index)
		{
			if (_Text.CharacterSize == 0) return;

			_Lines.Clear();
			string s = Text;
			_Text.DisplayedString = Text;
			float LastMod = 0;

			int LastSpace = -1;

			string l = "";
			int li = 0;
			for (uint i = 0; i < s.Length; i++)
			{
				float thisMod = _Text.FindCharacterPos(i + 1).X % Width;

				if (s[(int)i] == ' ' || s[(int)i] == '\n') LastSpace = (int)i;

				if (thisMod < LastMod && s[(int)i] != '\n')
				{
					if (LastSpace > -1 && LastSpace != i && s[LastSpace] == ' ' && _Text.FindCharacterPos((uint)LastSpace).X % Width > thisMod)
					{
						int lsli = (int)(li - i + LastSpace);
						if (lsli < 0) lsli = li;
						_Lines.Add(l.Substring(0, lsli + 1));
						l = l.Substring(lsli + 1);
						li = l.Length - 1;
					}
				}
				else if (s[(int)i] == '\n')
				{
					_Lines.Add(l);
					l = "";
					li = 0;
				}
				if (s[(int)i] != '\n')
				{
					LastMod = thisMod;
					l += s[(int)i];
					li++;
				}
			}
			_Lines.Add(l);
			CreateStringFromLines(Index, Height);
		}

		private void CreateStringFromLines(uint Index, int Height)
		{
			int l = (int)(Height / (_Text.CharacterSize * 1.25));
			_Text.DisplayedString = String.Join("\n", _Lines.Skip((int)Index).Take(l));
		}

		public void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			_Text.Position = Transform * _Position;
			Target.Draw(_Text);
		}

		public void PerformTransitions(Dictionary<string, float> Transitions, SubClass From, SubClass To)
		{
			Color FromC = (Color)From.GetAttributeWithDefault("font-color", ClassLibrary.NullColor);
			Color ToC = (Color)To.GetAttributeWithDefault("font-color", ClassLibrary.NullColor);
			float T = (float)To.GetAttributeWithDefault(Transitions, "transition-font-color", "transition", 1);

			_Text.Color = Cardamom.Utilities.ColorMath.BlendColors(FromC, ToC, T);
			if (From.GetAttributeWithDefault("font-face", null) != To.GetAttributeWithDefault("font-face", null))
			{
				_Text.Font = (Font)To.GetAttributeWithDefault("font-face", null);
			}

			int FromSize = (int)From.GetAttributeWithDefault("font-size", 0);
			int ToSize = (int)To.GetAttributeWithDefault("font-size", 0);
			T = (float)To.GetAttributeWithDefault(Transitions, "transition-font-size", "transition", 1);
			_Text.CharacterSize = (uint)(FromSize * (1 - T) + ToSize * T);

			int FromWidth = (int)From.GetAttributeWithDefault("width", 0);
			int ToWidth = (int)To.GetAttributeWithDefault("width", 0);
			T = (float)To.GetAttributeWithDefault(Transitions, "transition-width", "transition", 1);
			_Width = (int)(FromWidth * (1 - T) + ToWidth * T);

			int FromHeight = (int)From.GetAttributeWithDefault("height", 0);
			int ToHeight = (int)To.GetAttributeWithDefault("height", 0);
			T = (float)To.GetAttributeWithDefault(Transitions, "transition-height", "transition", 1);
			_Height = (int)(FromHeight * (1 - T) + ToHeight * T);

			int[] FromPadding = (int[])From.GetAttributeWithDefault("padding", ClassLibrary.NullArray);
			int[] ToPadding = (int[])To.GetAttributeWithDefault("padding", ClassLibrary.NullArray);
			T = (float)To.GetAttributeWithDefault(Transitions, "transition-padding", "transition", 1);
			_Position = new Vector2f(FromPadding[0] * (1 - T) + ToPadding[0] * T, FromPadding[1] * (1 - T) + ToPadding[1] * T);

			_PaddedWidth = (int)(_Width - _Position.X - FromPadding[2] * (1 - T) - ToPadding[2] * T);
			_PaddedHeight = (int)(_Height - _Position.Y - FromPadding[3] * (1 - T) - ToPadding[3] * T);

			SetFormatedString(_String, _PaddedWidth, _PaddedHeight, _Index);
		}
	}
}
