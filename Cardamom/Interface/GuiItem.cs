using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;
using NLua;

namespace Cardamom.Interface
{
	public abstract class GuiItem : Pod
	{
		private delegate void UpdateEventHandler(object Sender, UpdateEventArgs E);
		private event UpdateEventHandler OnUpdate;

		private Vector2f _Position;
		private bool _Visible;

		public Vector2f Position { get { return _Position; } set { _Position = value; } }
		public virtual bool Visible { get { return _Visible; } set { _Visible = value; } }
		public abstract Vector2f Size { get; }

		public GuiItem() { _Visible = true; }

		public GuiItem(System.Xml.XmlReader Reader)
		{
			string Code = Reader.GetAttribute("position");
			if (Code != String.Empty) _Position = ClassLibrary.Instance.ParseVector2f(Code);

			Code = Reader.GetAttribute("visible");
			if (Code != String.Empty) _Visible = Convert.ToBoolean(Code);
		}

		public void SetPosition(double x, double y) { _Position.X = (float)x; _Position.Y = (float)y; }
		public virtual void AddEventScript(string EventType, LuaFunction Event)
		{
			if (EventType.ToLower() == "update") OnUpdate += new UpdateEventHandler(delegate (object s, UpdateEventArgs e) { Event.Call(s, e); });
		}

		public virtual void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			if (OnUpdate != null) OnUpdate(this, new UpdateEventArgs(MouseController, KeyController, DeltaT, Transform));
		}

		public abstract void Draw(RenderTarget Target, Transform Transform);
	}
}
