using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;

using Cardamom.Planar;
using Cardamom.Interface.Items.Subcomponents;

namespace Cardamom.Interface.Items
{
	public class Slider : ClassedGuiInput<double>
	{
		RectComponent _Back;
		SliderKnob _Knob;

		private Polygon _Box;

		private bool _Sliding;

		private double _Min;
		private double _Max = 1;

		public override Vector2f Size { get { return new Vector2f(_Box[2].Point.X, _Box[2].Point.Y); } }
		public double Min { get { return _Min; } set { _Min = value; Valuing(); } }
		public double Max { get { return _Max; } set { _Max = value; Valuing(); } }
		new public double Value { get { return _Value; } set { _Value = value; Valuing(); } }

		public Slider(string BackClass, string KnobClass)
			: base(BackClass, Series.NoFocus)
		{
			_Back = new RectComponent(_Class);
			_Knob = new SliderKnob(KnobClass);
			_Knob.Parent = this;
			_Box = _Back.GetBoundingBox();
			int[] P = (int[])_Class.GetAttributeWithDefault("padding", ClassLibrary.NullArray);
			_Knob.Position = new Vector2f(P[0], P[1]);
		}

		public override bool IsCollision(Vector2f Point)
		{
			return _Box.ContainsPoint(Point);
		}

		public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			Transform.Translate(Position);
			base.Update(MouseController, KeyController, DeltaT, Transform);
			_Knob.Update(MouseController, KeyController, DeltaT, Transform);

			if ((_Knob.IsCollision(MouseController.Position) || _Sliding) && MouseController.LeftDown)
			{
				_Sliding = true;
				_Knob.Position += new Vector2f(MouseController.DragDelta.X, 0);
				Positioning();
				if (OnChange != null) OnChange(this, new ValueChangedEventArgs<double>(Value));
			}
			else if (_Sliding && !MouseController.LeftDown) _Sliding = false;
		}

		private void Positioning()
		{
			int[] P = (int[])_Class.GetAttributeWithDefault("padding", ClassLibrary.NullArray);
			float Left = P[0];
			float Right = _Back.Size.X - P[2] - _Knob.Size.X;
			if (_Knob.Position.X < Left) _Knob.Position = new Vector2f(Left, P[1]);
			else if (_Knob.Position.X > Right) _Knob.Position = new Vector2f(Right, P[1]);
			double v = (_Knob.Position.X - Left) / (Right - Left);
			_Value = _Min + v * (_Max - _Min);
		}

		private void Valuing()
		{
			if ((double)_Value > _Max) _Value = _Max;
			else if ((double)_Value < _Min) _Value = _Min;

			int[] P = (int[])_Class.GetAttributeWithDefault("padding", ClassLibrary.NullArray);
			float Left = P[0];
			float Right = _Back.Size.X - P[2] - _Knob.Size.X;

			double v = ((double)_Value - _Min) / (_Max - _Min);

			_Knob.Position = new Vector2f((float)(Left + (Right - Left) * v), P[1]);
		}

		public override void Draw(SFML.Graphics.RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			_Back.Draw(Target, Transform);
			_Knob.Draw(Target, Transform);
		}

		public override void PerformTransitions(Dictionary<string, float> Transitions)
		{
			_Back.PerformTransitions(Transitions, _Class[_LastMode], _Class[_Mode]);
		}
	}
}
