using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;

using SFML.Window;
using SFML.Graphics;

namespace Cardamom.Interface.Items
{
	public abstract class GuiSerialContainer<T> : GuiConstruct<T>, IEnumerable<T> where T : ClassedGuiItem
	{
		bool _Foldable;
		bool _FoldedOpen;
		bool _Vertical;


		protected List<T> _Items = new List<T>();

		public IEnumerator<T> GetEnumerator() { return _Items.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		public GuiSerialContainer(string ClassName, bool Foldable = false, bool Vertical = true)
			: base(ClassName, Series.NoFocus)
		{
			RectComponent R = new RectComponent(_Class);
			_Box = R.GetBoundingBox();
			_Components = new Component[] { R };

			_Foldable = Foldable;
			_Vertical = Vertical;
			_FoldedOpen = !Foldable;

			if (_Foldable)
			{
				OnMouseOver += (S, E) => _FoldedOpen = Mode == Class.Mode.Focus;
				OnMouseOut += (S, E) => _FoldedOpen = false;
				OnClick += (S, E) => _FoldedOpen = true;
			}
		}

		public virtual void Add(T Item)
		{
			_Items.Add(Item);
			Item.Parent = this;
		}

		public virtual void Clear()
		{
			_Items.Clear();
		}

		private Vector2f ItemTranslation(T Item)
		{
			if (_Vertical) return new Vector2f(0, Item.Size.Y);
			else return new Vector2f(Item.Size.X, 0);
		}

		public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);
			Transform.Translate(Position + LeftPadding);

			Vector2f H = new Vector2f(0, 0);
			foreach (T Item in _Items)
			{
				Item.Visible = _FoldedOpen;
				Item.Update(MouseController, KeyController, DeltaT, Transform);
				Vector2f translateBy = ItemTranslation(Item);
				Transform.Translate(translateBy);
				if (_FoldedOpen) H += translateBy;
			}
			_Box.SetSize(new Vector2f(Math.Max(_Box.Size.X, H.X), Math.Max(_Box.Size.Y, H.Y)));
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);
			Transform.Translate(Position + LeftPadding);
			if (_FoldedOpen)
			{
				foreach (T Item in _Items)
				{
					Item.Draw(Target, Transform);
					Transform.Translate(ItemTranslation(Item));
				}
			}
		}
	}
}
