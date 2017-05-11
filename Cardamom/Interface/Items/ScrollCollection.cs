using System;

namespace Cardamom.Interface.Items
{
	public class ScrollCollection<T> : GuiSerialContainer<ClassedGuiInput<T>>
	{
		public ScrollCollection(string ClassName)
			: base(ClassName, false) { }
	}
}
