using System;
namespace Cardamom.Interface.Items
{
	public class SingleColumnTable : GuiSerialContainer<ClassedGuiItem>
	{
		public SingleColumnTable(string ClassName) : base(ClassName, false, true) { }	}
}
