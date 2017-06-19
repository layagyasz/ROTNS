using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace Cardamom.Interface
{
	public class Container<T> : Pod, IEnumerable<T> where T : Pod
	{
		public bool Visible = true;

		protected List<T> _Items = new List<T>();

		public List<T> Items { get { return _Items; } }

		public IEnumerator<T> GetEnumerator() { return _Items.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		public virtual void Add(T Item) { _Items.Add(Item); }
		public void Remove(T Item) { _Items.Remove(Item); }
		public void Clear() { _Items.Clear(); }

		public virtual void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			if (Visible)
				_Items.ToList().ForEach(Item => Item.Update(MouseController, KeyController, DeltaT, Transform));
		}

		public virtual void Draw(RenderTarget Target, Transform Transform)
		{
			if (Visible)
				_Items.ToList().ForEach(Item => Item.Draw(Target, Transform));
		}
	}
}
