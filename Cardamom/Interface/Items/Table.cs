using System;

namespace Cardamom.Interface.Items
{
	public class Table : GuiSerialContainer<TableRow>
	{
		public Table(string ClassName) : base(ClassName, false, true) { }
	}
}
