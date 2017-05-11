using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using ROTNS.Model;
using ROTNS.View;

using Venetia;

namespace ROTNS.View
{
    class InteractionController
    {
        WorldView _Map;
        Economy _Economy;
        IEnumerable<Tangible> _CurrentItem;

        //Panes
        RegionViewWindow _RegionViewWindow = new RegionViewWindow("window");

        public InteractionController(WorldView Map, Economy Economy, Screen Screen)
        {
            _Map = Map;
            _Map.ChangeView(R => R.ChangeViewByPoint(M => M.Biome.Color, true, true));
            _Economy = Economy;
            _CurrentItem = _Economy.All;
            _Map.OnClick += new EventHandler<MouseEventArgs>(ClickedMap);
            _Map.OnRightClick += new EventHandler<MouseEventArgs>(RightClickedMap);
            _RegionViewWindow.Visible = false;

            //Buttons
            string[] Classes = new string[]
            {
                "button-biome",
                "button-temperature",
                "button-moisture",
                "button-elevation",
                "button-population",
                "button-wealth",
                "button-culture",
                "button-nation",
                "button-good"
            };
            Action<object, MouseEventArgs>[] Actions = new Action<object, MouseEventArgs>[]
            {
                delegate(object S, MouseEventArgs E) { _Map.ChangeView(R => R.ChangeViewByPoint(M => M.Biome.Color, true, true));},
                delegate(object S, MouseEventArgs E) { _Map.ChangeView(R => R.ChangeViewByPoint(M => M.Temperature, Color.Blue, Color.Red));},
                delegate(object S, MouseEventArgs E) { _Map.ChangeView(R => R.ChangeViewByPoint(M => M.Moisture,  Color.Yellow, Color.Blue));},
                delegate(object S, MouseEventArgs E) { _Map.ChangeView(R => R.ChangeViewByPoint(M => M.Height, Color.Green, Color.White));},
                delegate(object S, MouseEventArgs E) { _Map.ChangeView(R => R.Region.PopulationDensity, Color.White, Color.Red); },
                delegate(object S, MouseEventArgs E) { _Map.ChangeView(R => (float)R.Region.FlowPerCapita(), Color.White, Color.Red); },
                delegate(object S, MouseEventArgs E) { },
                delegate(object S, MouseEventArgs E) { },
                delegate(object S, MouseEventArgs E) { }
            };

            for (int i = 0; i < Classes.Length; ++i)
            {
                Button Button = new Button(Classes[i]);
                Button.Position = new SFML.Window.Vector2f(34 * i + 10, 0);
                Button.OnClick += new EventHandler<MouseEventArgs>(Actions[i]);
                Screen.Add(Button);
            }

            Screen.Add(_RegionViewWindow);
        }

        private void ClickedMap(object Sender, MouseEventArgs E)
        {
            _RegionViewWindow.SetRegion(_Map, _Map.GetRegionAt(E.Position));
            _RegionViewWindow.Visible = true;
        }

        private void RightClickedMap(object Sender, MouseEventArgs E)
        {
            _RegionViewWindow.Visible = false;
        }
    }
}
