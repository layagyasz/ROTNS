using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;

using Cardamom.Utilities;
using Cardamom.Planar;

namespace Cardamom.Interface
{
    public abstract class Interactive : GuiItem
    {
        Interactive _Parent;
        List<Interactive> _Links;

        private bool _Enabled;
        private bool _Right;
        private bool _Hover;

        private bool _HoverAck;

        public EventHandler<MouseEventArgs> Clicked;
        public EventHandler<MouseEventArgs> RightClicked;
        public EventHandler<MouseEventArgs> MouseOver;
        public EventHandler<MouseEventArgs> MouseOut;
        public EventHandler<MouseEventArgs> Leave;

        public Interactive Parent { get { return _Parent; } set { _Parent = value; } }
        public bool Hover { get { return _Hover; } }
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }

        public Interactive()
        {
            _Enabled = true;
        }

        public Interactive(System.Xml.XmlReader Reader) : base(Reader) { }

        internal void Acknowledge(MouseController.RequestType Request, Vector2f Position, bool Primary = true)
        {
            if (_Enabled)
            {
                if (_Links != null)
                {
                    foreach (Interactive Link in _Links) Link.Acknowledge(Request, Position, false);
                }
                if (_Parent != null) _Parent.Acknowledge(Request, Position, false);
                switch (Request)
                {
                    case MouseController.RequestType.Focus:
                        if (!_Right)
                        {
                            if (Clicked != null) Clicked(this, new MouseEventArgs(Position));
                        }
                        else
                        {
                            if (RightClicked != null) RightClicked(this, new MouseEventArgs(Position));
                        }
                        break;
                    case MouseController.RequestType.Leave:
                        if (Primary && Leave != null) Leave(this, new MouseEventArgs(Position));
                        break;
                    case MouseController.RequestType.MouseOut:
                        if (Primary && MouseOut != null)
                        {
                            MouseOut(this, new MouseEventArgs(Position));
                            _HoverAck = false;
                        }
                        break;
                    case MouseController.RequestType.MouseOver:
                        if ((Primary || !_HoverAck) && MouseOver != null) MouseOver(this, new MouseEventArgs(Position));
                        _HoverAck = true;
                        break;
                }
            }
        }

        public void AddLink(Interactive Mouseable)
        {
            if (_Links == null) _Links = new List<Interactive>();
            _Links.Add(Mouseable);
        }

        public override void AddEventScript(string EventType, NLua.LuaFunction Event)
        {
            EventHandler<MouseEventArgs> E = new EventHandler<MouseEventArgs>(delegate(object s, MouseEventArgs e) { Event.Call(this, e); });
            switch (EventType.ToLower())
            {
                case "click": Clicked += E; break;
                case "right-click": RightClicked += E; break;
                case "mouse-over": MouseOver += E; break;
                case "mouse-out": MouseOut += E; break;
                case "leave": Leave += E; break;
                case "update": base.AddEventScript("update", Event); break;
            }
        }

        public abstract bool IsCollision(Vector2f Point);

        public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, PlanarTransformMatrix Transform)
        {
            base.Update(MouseController, KeyController, DeltaT, Transform);
            bool m = false;

            if (Visible && _Enabled)
            {
                m = IsCollision(MouseController.Position);

                if ((!_Hover && m) || (_Hover && !_HoverAck)) MouseController.Queue(this, MouseController.RequestType.MouseOver);
                else if (_Hover && m) MouseController.Queue(this, MouseController.RequestType.Block);
                if (m && (MouseController.LeftClick || MouseController.RightClick)) MouseController.Queue(this, MouseController.RequestType.Focus);
            }
            else _Hover = false;
            _Right = MouseController.RightClick;
            if (!m && _HoverAck) MouseController.Queue(this, MouseController.RequestType.MouseOut);
            if (!m && MouseController.LeftClick) MouseController.Queue(this, MouseController.RequestType.Leave);
            _Hover = m;
        }
    }
}
