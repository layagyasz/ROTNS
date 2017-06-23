using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;

using SFML.Window;
using SFML.Graphics;

namespace Cardamom.Interface
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
			: base(ClassName, Series.Standard)
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
			Item.OnClick += (sender, e) => HandleClick((T)sender);
			Item.Parent = this;
		}

		protected virtual void HandleClick(T Item)
		{
			if (Value != null) Value.Mode = Class.Mode.None;
			Value = Item;
		}

		public virtual void Remove(T Item)
		{
			_Items.Remove(Item);
			if (Item == Value) Value = null;
		}

		public virtual void Remove(Func<T, bool> Predicate)
		{
			IEnumerable<T> toRemove = _Items.Where(Predicate);
			if (toRemove.Contains(Value)) Value = null;
			_Items.RemoveAll(i => Predicate(i));
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

			if (!Visible) return;

			Transform.Translate(Position + LeftPadding);

			Vector2f H = new Vector2f(0, 0);
			bool draw = true;
			foreach (T Item in _Items)
			{
				Item.Visible = draw;
				Item.Update(MouseController, KeyController, DeltaT, Transform);
				Vector2f translateBy = ItemTranslation(Item);
				Transform.Translate(translateBy);
				if (draw) H += translateBy;
				draw = _FoldedOpen;
			}

			if (_Vertical) _Box.SetSize(new Vector2f(_Box.Size.X, H.Y));
			else _Box.SetSize(new Vector2f(H.X, _Box.Size.Y));
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);

			if (!Visible) return;

			Transform.Translate(Position + LeftPadding);
			if (_FoldedOpen)
			{
				foreach (T Item in _Items)
				{
					Item.Draw(Target, Transform);
					Transform.Translate(ItemTranslation(Item));
				}
			}
			else if (_Items.Count > 0) _Items[0].Draw(Target, Transform);
		}
	}
}
