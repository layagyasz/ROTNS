using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Interface.Items
{
	public class Select<T> : GuiSerialContainer<StandardItem<T>>
	{
		public Select(string ClassString, bool Foldable = true, bool Vertical = true)
			: base(ClassString, Foldable, Vertical) { }

		public override void Add(StandardItem<T> Item)
		{
			base.Add(Item);
			UpdateDummy();
		}

		public override void Remove(Func<StandardItem<T>, bool> Predicate)
		{
			base.Remove(Predicate);
			UpdateDummy();
		}

		public override void Remove(StandardItem<T> Item)
		{
			base.Remove(Item);
			UpdateDummy();
		}

		protected override void HandleClick(StandardItem<T> Item)
		{
			base.HandleClick(Item);

			if (_Foldable)
			{
				_Items[0] = new SelectionDummyOption<T>(Item) { Parent = this };
				_Items[0].Mode = Class.Mode.Focus;
			}
		}

		private void UpdateDummy()
		{
			if (_Foldable)
			{
				if (_Items.Count == 1 && !(_Items[0] is SelectionDummyOption<T>))
				{
					_Items.Insert(0, new SelectionDummyOption<T>(_Items[0]) { Parent = this });
				}
				else if (_Items.Count > 1) _Items[0] = new SelectionDummyOption<T>(_Items[1]) { Parent = this };
				else _Items.Clear();

				if (_Items.Count > 1) Value = _Items[1];
			}
			else if (_Items.Count > 0) Value = _Items[0];
		}
	}
}
