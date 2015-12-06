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

        Pair<Interactive, RequestType> _Changes;

        private Vector2f _DragStart;
        private Vector2f _DragDelta;

        private bool _LeftPressed;
        private bool _LeftDrag;
        private bool _LeftClick;
        private bool _LeftDown;
        private bool _LeftUp;

        private bool _RightClick;
        private bool _RightDown;
        private bool _RightUp;

        private int _WheelDelta;

        private Vector2f _Position;

        public Vector2f Position { get { return _Position; } }
        public int WheelDelta { get { return _WheelDelta; } }
        public bool LeftClick { get { return _LeftClick; } }
        public bool RightClick { get { return _RightClick; } }

        public MouseController(RenderWindow Window)
        {
            Window.MouseWheelMoved += new EventHandler<MouseWheelEventArgs>(Scroll);
            _Changes = new Pair<Interactive, RequestType>();
        }

        private void Scroll(object sender, EventArgs e)
        {
            _WheelDelta = ((MouseWheelEventArgs)e).Delta;
        }

        public void Update(RenderWindow Window)
        {
            _WheelDelta = 0;

            if (_Changes.First != null)
            {
                _Changes.First.Acknowledge(_Changes.Second, _Position);
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
                _DragDelta = Position - _DragStart;
                if (_LeftDown && Math.Abs(_DragDelta.X) + Math.Abs(_DragDelta.Y) >= 5 && on)
                {
                    _LeftDrag = true;
                    _DragStart = Position;
                }
            }
            else { _DragDelta = new Vector2f(0, 0); }

            _LeftUp = !Mouse.IsButtonPressed(Mouse.Button.Left);
            _LeftClick = _LeftUp && _LeftDown && !_LeftDrag;
            _LeftPressed = !_LeftDown && !_LeftUp;
            if (_LeftPressed)
            {
                _DragStart = Position;
            }
            _LeftDown = !_LeftUp;
            if (_LeftUp) _LeftDrag = false;

            _RightUp = !Mouse.IsButtonPressed(Mouse.Button.Right);
            _RightClick = _RightUp && _RightDown;
            _RightDown = !_RightUp;
        }

        internal void Queue(Interactive Mouseable, RequestType Type)
        {
            switch (Type)
            {
                case RequestType.Focus:
                    _Changes.First = Mouseable;
                    _Changes.Second = Type;
                    break;
                case RequestType.Leave:
                    Mouseable.Acknowledge(Type, _Position);
                    break;
                case RequestType.MouseOut:
                    Mouseable.Acknowledge(Type, _Position);
                    break;
                case RequestType.MouseOver:
                    _Changes.First = Mouseable;
                    _Changes.Second = Type;
                    break;
                case RequestType.Block:
                    _Changes.First = null;
                    break;
            }
        }
    }
}
