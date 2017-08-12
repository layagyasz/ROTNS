using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;

using Cardamom.Interface.Items.Subcomponents;
using Cardamom.Planar;
using Cardamom.Utilities;

namespace Cardamom.Interface
{
	public abstract class ClassedGuiInput<T> : ClassedGuiItem
	{
		public EventHandler<ValuedEventArgs<T>> OnChange;

		protected T _Value;

		public T Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
				if (OnChange != null) OnChange(this, new ValuedEventArgs<T>(value));
			}
		}

		public ClassedGuiInput(string ClassName, Series Series = Series.Standard)
			: base(ClassName, Series) { }
	}
}
