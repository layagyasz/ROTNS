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
	public class ScrollCollection<T> : GuiConstruct<ClassedGuiInput<T>>, IEnumerable<ClassedGuiInput<T>>
	{
		List<ClassedGuiInput<T>> _Items = new List<ClassedGuiInput<T>>();
		int _StartIndex;

		public IEnumerator<ClassedGuiInput<T>> GetEnumerator() { return _Items.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		public ScrollCollection(string ClassName)
			: base(ClassName, Series.Standard)
		{
			RectComponent R = new RectComponent(_Class);
			_Box = R.GetBoundingBox();
			_Components = new Component[] { R };
		}

		public virtual void Add(ClassedGuiInput<T> Item)
		{
			_Items.Add(Item);
			Item.OnClick += (sender, e) => HandleClick((ClassedGuiInput<T>)sender);
			Item.Parent = this;
		}

		public void Insert(int Index, ClassedGuiInput<T> Item)
		{
			_Items.Insert(Index, Item);
			Item.OnClick += (sender, e) => HandleClick((ClassedGuiInput<T>)sender);
			Item.Parent = this;
		}

		public void Move(int Index, Func<ClassedGuiInput<T>, bool> Predicate)
		{
			IEnumerable<ClassedGuiInput<T>> toMove = _Items.Where(Predicate);
			_Items.RemoveAll(i => Predicate(i));
			foreach (var item in toMove) _Items.Insert(Index, item);
		}

		public void Move(int Index, ClassedGuiInput<T> Item)
		{
			_Items.Remove(Item);
			_Items.Insert(Index, Item);
		}

		protected void HandleClick(ClassedGuiInput<T> Item)
		{
			if (Value != null) Value.Mode = Class.Mode.None;
			Value = Item;
		}

		public void Remove(ClassedGuiInput<T> Item)
		{
			_Items.Remove(Item);
			if (Item == Value) Value = null;
		}

		public void Remove(Func<ClassedGuiInput<T>, bool> Predicate)
		{
			IEnumerable<ClassedGuiInput<T>> toRemove = _Items.Where(Predicate);
			if (toRemove.Contains(Value)) Value = null;
			_Items.RemoveAll(i => Predicate(i));
		}

		public virtual void Clear()
		{
			_Items.Clear();
		}

		private Vector2f ItemTranslation(ClassedGuiInput<T> Item)
		{
			return new Vector2f(0, Item.Size.Y);
		}

		public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);

			if (!Visible) return;

			if (Hover)
			{
				_StartIndex -= MouseController.WheelDelta;
				_StartIndex = Math.Max(0, Math.Min(_Items.Count - 1, _StartIndex));
			}

			Transform.Translate(Position + LeftPadding);

			Vector2f H = new Vector2f(0, 0);
			bool draw = true;
			foreach (ClassedGuiInput<T> Item in _Items.Skip(_StartIndex))
			{
				Vector2f translateBy = ItemTranslation(Item);
				H += translateBy;
				draw = IsCollision(H);
				Item.Visible = draw;

				if (draw)
				{
					Item.Update(MouseController, KeyController, DeltaT, Transform);
					Transform.Translate(translateBy);
				}
			}
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);

			if (!Visible) return;

			Transform.Translate(Position + LeftPadding);

			Vector2f H = new Vector2f(0, 0);
			bool draw = true;
			foreach (ClassedGuiInput<T> Item in _Items.Skip(_StartIndex))
			{
				Vector2f translateBy = ItemTranslation(Item);
				H += translateBy;
				draw = IsCollision(H);
				Item.Visible = draw;

				if (draw)
				{
					Item.Draw(Target, Transform);
					Transform.Translate(ItemTranslation(Item));
				}
			}
		}
	}
}
