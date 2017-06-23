using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

using Cardamom.Interface.Items.Subcomponents;
using Cardamom.Planar;

namespace Cardamom.Interface.Items
{
	public class Pane : GuiContainer<Pod>
	{
		Polygon _DragBox;

		public Polygon DragBox { get { return _DragBox; } set { _DragBox = value; } }

		public Pane(string ClassName)
			: base(ClassName)
		{
		}

		public override void Add(Pod Item)
		{
			base.Add(Item);
			if (Item is Interactive)
			{
				((Interactive)Item).Parent = this;
			}
		}

		public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);
			if (Hover)
			{
				Position += MouseController.DragDelta;
			}
		}
	}
}
