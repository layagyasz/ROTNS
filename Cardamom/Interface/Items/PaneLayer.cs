using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Cardamom.Interface.Items
{
	public class PaneLayer : Container<Pane>
	{
		public PaneLayer()
		{
		}

		public override void Add(Pane Pane)
		{
			base.Add(Pane);
			Pane.OnClick += FocusPane;
		}

		private void FocusPane(object sender, MouseEventArgs e)
		{
			_Items.Remove((Pane)sender);
			_Items.Add((Pane)sender);
		}
	}
}
