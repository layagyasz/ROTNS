using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

using Cardamom.Interface.Items.Subcomponents;

namespace Cardamom.Interface
{
	public class GuiContainer<T> : GuiConstruct<T>, IEnumerable<T> where T : Pod
	{
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
				foreach (Pod p in _Items)
					if (p is GuiItem) ((GuiItem)p).Visible = value;
			}
		}

		protected List<T> _Items = new List<T>();

		public IEnumerator<T> GetEnumerator() { return _Items.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		public GuiContainer(string ClassName)
			: base(ClassName, Series.NoFocus)
		{
			RectComponent R = new RectComponent(_Class);
			_Box = R.GetBoundingBox();
			_Components = new Component[] { R };
		}

		public virtual void Add(T Item) { _Items.Add(Item); }
		public void Remove(T Item) { _Items.Remove(Item); }

		public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);
			Transform.Translate(Position + LeftPadding);
			_Items.ForEach(Item => Item.Update(MouseController, KeyController, DeltaT, Transform));
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);
			Transform.Translate(Position + LeftPadding);
			if (Visible) _Items.ForEach(Item => { Item.Draw(Target, Transform); });
		}
	}
}
