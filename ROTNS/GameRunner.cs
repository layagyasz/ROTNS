using System;
using System.Collections.Generic;

using SFML.Window;

using Cardamom.Interface;

using ROTNS.Model;
using ROTNS.View;

namespace ROTNS
{
	public class GameRunner
	{
		Interface _Interface;

		World _World;
		WorldView _WorldView;

		TickManager _TickManager = new TickManager();
		Dictionary<Ticked, GuiItem> _MapItems = new Dictionary<Ticked, GuiItem>();

		public GameRunner(Interface Interface)
		{
			_Interface = Interface;
			_Interface.Screen = new Screen();
			_TickManager.OnRemove += (sender, e) => RemoveTicked((Ticked)sender);
		}

		public void SetWorld(World World)
		{
			_World = World;
			_WorldView = new WorldView(
				World, new Vector2f(_Interface.Window.Size.X, _Interface.Window.Size.Y - 24));
			_Interface.Screen.Add(_WorldView);
			InteractionController I = new InteractionController(_WorldView, World.Settings.Economy, _Interface.Screen);
		}

		public void AddTicked(Ticked Ticked)
		{
			_TickManager.Add(Ticked);
		}

		public void AddTicked(Ticked Ticked, GuiItem View)
		{
			_TickManager.Add(Ticked);
			_MapItems.Add(Ticked, View);
			_WorldView.AddOverlay(View);
		}

		public void RemoveTicked(Ticked Ticked)
		{
			_TickManager.Remove(Ticked);
			if (_MapItems.ContainsKey(Ticked))
			{
				_WorldView.RemoveOverlay(_MapItems[Ticked]);
				_MapItems.Remove(Ticked);
			}
		}

		public void Start()
		{
			_TickManager.Start();
			_Interface.Start(false, true);
		}
	}
}
