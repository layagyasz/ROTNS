using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;
using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

namespace Cardamom.Interface
{
	public abstract class GuiConstruct<T> : ClassedGuiInput<T>
	{
		protected Component[] _Components;
		protected Rectangle _Box;

		int[] _Padding = new int[4];
		int[] _Margin = new int[4];

		public override Vector2f Size { get { return new Vector2f(_Box[2].Point.X + _Margin[0] + _Margin[2], _Box[2].Point.Y + _Margin[1] + _Margin[3]); } }
		public Vector2f LeftPadding { get { return new Vector2f(_Padding[0], _Padding[1]); } }
		public Vector2f LeftMargin { get { return new Vector2f(_Margin[0], _Margin[1]); } }

		public GuiConstruct(string ClassName, Series Series = Series.Standard)
			: base(ClassName, Series)
		{
			_Padding = (int[])_Class.GetAttributeWithDefault("padding", ClassLibrary.NullArray);
			_Margin = (int[])_Class.GetAttributeWithDefault("margin", ClassLibrary.NullArray);
		}

		public override bool IsCollision(Vector2f Point)
		{
			return _Box.ContainsPoint(Point);
		}

		public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			Transform.Translate(Position + LeftMargin);
			base.Update(MouseController, KeyController, DeltaT, Transform);
			Array.ForEach(_Components, Component => Component.Update(MouseController, KeyController, DeltaT, Transform));
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position + LeftMargin);
			if (Visible) Array.ForEach(_Components, Component => Component.Draw(Target, Transform));
		}

		public override void PerformTransitions(Dictionary<string, float> Transitions)
		{
			Array.ForEach(_Components, Component => Component.PerformTransitions(Transitions, _Class[_LastMode], _Class[_Mode]));

			int[] FromP = (int[])_Class[_LastMode].GetAttributeWithDefault("padding", ClassLibrary.NullArray);
			int[] ToP = (int[])_Class[_Mode].GetAttributeWithDefault("padding", ClassLibrary.NullArray);
			float T = _Class[_Mode].GetAttributeWithDefault(Transitions, "transition-padding", "transition", 1);
			for (int i = 0; i < 4; ++i) _Padding[i] = FromP[i] + (int)(T * (ToP[i] - FromP[i]));

			FromP = (int[])_Class[_LastMode].GetAttributeWithDefault("margin", ClassLibrary.NullArray);
			ToP = (int[])_Class[_Mode].GetAttributeWithDefault("margin", ClassLibrary.NullArray);
			T = _Class[_Mode].GetAttributeWithDefault(Transitions, "transition-margin", "transition", 1);
			for (int i = 0; i < 4; ++i) _Margin[i] = FromP[i] + (int)(T * (ToP[i] - FromP[i]));
		}
	}
}
