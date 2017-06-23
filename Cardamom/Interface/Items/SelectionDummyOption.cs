using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Interface.Items
{
	class SelectionDummyOption<T> : StandardItem<T>
	{
		public SelectionDummyOption(string ClassName)
			: base(ClassName, Series.Standard) { }

		public SelectionDummyOption(StandardItem<T> Clone)
			: base(Clone.Class.Name, Series.Standard)
		{
			DisplayedString = Clone.DisplayedString;
			Value = Clone.Value;
		}
	}
}
