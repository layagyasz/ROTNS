using System;
using System.Collections.Generic;

namespace Cardamom.Interface.Items
{
	public class TableRow : GuiSerialContainer<ClassedGuiItem>
	{
		public TableRow(string ClassName) : base(ClassName, false, false) { }
	}
}
