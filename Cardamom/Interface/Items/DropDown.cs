using System;
namespace Cardamom.Interface.Items
{
	public class DropDown<T> : GuiSerialContainer<StandardItem<T>>
	{
		public DropDown(string ClassName)
			: base(ClassName, true)
		{
		}
	}
}
