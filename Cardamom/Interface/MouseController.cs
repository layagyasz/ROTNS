using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;
using SFML.Window;
using SFML.Graphics;

namespace Cardamom.Interface
{
	public class MouseController
	{
		internal enum RequestType { MouseOver, MouseOut, Focus, Leave, Block }

		public EventHandler<MouseEventArgs> OnLeftClick;
		public EventHandler<MouseEventArgs> OnLeftDown;
		public EventHandler<MouseEventArgs> OnLeftUp;

		Triplet<Interactive, RequestType, Vector2f> _Changes;

		private Vector2f _DragStart;
		private Vector2f _DragDelta;

		private bool _LeftDrag;
		private bool _LeftClick;
		private bool _LeftDown;
		private bool _LeftUp = true;

		private bool _RightClick;
		private bool _RightDown;
		private bool _RightUp = true;

		private int _WheelDelta;
		private bool _WheelMoved;

		private Vector2f _Position;

		private Interactive _Top;
		private Interactive _LastTop;

		public Vector2f Position { get { return _Position; } }
		public Interactive Top { get { return _LastTop; } }
		public int WheelDelta { get { return _WheelDelta; } }
		public bool LeftDown { get { return _LeftDown; } }
		public bool RightDown { get { return _RightDown; } }
		public bool LeftClick { get { return _LeftClick; } }
		public bool RightClick { get { return _RightClick; } }
		public Vector2f DragDelta { get { return _DragDelta; } }
		public bool LeftDrag { get { return _LeftDrag; } }

		public MouseController(RenderWindow Window)
		{
			Window.MouseWheelMoved += new EventHandler<MouseWheelEventArgs>(Scroll);
			_Changes = new Triplet<Interactive, RequestType, Vector2f>();
		}

		private void Scroll(object sender, EventArgs e)
		{
			_WheelDelta = ((MouseWheelEventArgs)e).Delta;
			_WheelMoved = true;
		}

		public void Update(RenderWindow Window)
		{
			if (!_WheelMoved) _WheelDelta = 0;
			_WheelMoved = false;

			if (_Changes.First != null)
			{
				_Changes.First.Acknowledge(_Changes.Second, _Changes.Third);
				_Changes.First = null;
			}

			_Position = Window.MapPixelToCoords(Mouse.GetPosition(Window), Window.GetView());
			bool on = true;
			if (Position.X < 0 || Position.Y < 0 || Position.Y > Window.Size.Y || Position.X > Window.Size.X) on = false;

			if (_LeftDrag && on)
			{
				_DragDelta = _Position - _DragStart;
				_DragStart = _Position;
			}
			else if (on)
			{
				_DragDelta = _Position - _DragStart;
				if (_LeftDown && Math.Abs(_DragDelta.X) + Math.Abs(_DragDelta.Y) >= 1)
				{
					_LeftDrag = true;
					_DragStart = Position;
				}
			}
			else { _DragDelta = new Vector2f(0, 0); }

			if (!_LeftUp && !Mouse.IsButtonPressed(Mouse.Button.Left)
				&& OnLeftUp != null) OnLeftUp(this, new MouseEventArgs(_Position));
			else if (!_LeftDown && Mouse.IsButtonPressed(Mouse.Button.Left)
				&& OnLeftDown != null) OnLeftDown(this, new MouseEventArgs(_Position));

			_LeftUp = !Mouse.IsButtonPressed(Mouse.Button.Left);
			_LeftClick = _LeftUp && _LeftDown && !_LeftDrag;

			if (_LeftClick && OnLeftClick != null) OnLeftClick(this, new MouseEventArgs(_Position));

			_DragStart = _Position;
			_LeftDown = !_LeftUp;
			if (_LeftUp)
			{
				_LeftDrag = false;
				_DragDelta = new Vector2f(0, 0);
			}

			_RightUp = !Mouse.IsButtonPressed(Mouse.Button.Right);
			_RightClick = _RightUp && _RightDown;
			_RightDown = !_RightUp;

			_LastTop = _Top;
		}

		internal void Queue(Interactive Mouseable, RequestType Type, Vector2f MousePosition)
		{
			switch (Type)
			{
				case RequestType.Focus:
					_Changes.First = Mouseable;
					_Changes.Second = Type;
					_Changes.Third = MousePosition;
					break;
				case RequestType.Leave:
					Mouseable.Acknowledge(Type, MousePosition);
					break;
				case RequestType.MouseOut:
					Mouseable.Acknowledge(Type, MousePosition);
					break;
				case RequestType.MouseOver:
					_Changes.First = Mouseable;
					_Changes.Second = Type;
					_Changes.Third = MousePosition;
					break;
				case RequestType.Block:
					_Changes.First = null;
					break;
			}
		}

		internal void Put(Interactive Mouseable) { _Top = Mouseable; }
	}
}
