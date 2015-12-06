using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

using Cardamom.Interface;

using Venetia;

namespace ROTNS.OverMapping
{
    class InteractionController
    {
        OverMap _Map;
        Economy _Economy;
        IEnumerator<Tangible> _CurrentItem;

        public InteractionController(OverMap Map, Economy Economy)
        {
            _Map = Map;
            _Economy = Economy;
            _CurrentItem = _Economy.All;
            _Map.Clicked += new EventHandler<MouseEventArgs>(ClickedMap);
            _Map.RightClicked += new EventHandler<MouseEventArgs>(RightClickedMap);
        }

        private void ClickedMap(object Sender, MouseEventArgs E)
        {
            if (!_CurrentItem.MoveNext()) { _CurrentItem = _Economy.All; _CurrentItem.MoveNext(); }
            Console.WriteLine(_CurrentItem.Current.Name);
            _Map.ChangeViewRanged(delegate(MapRegion Region) { return (float)Region.Region[_CurrentItem.Current].Price(Region.Region.Population); }, Color.White, Color.Red);
            //_Map.ChangeView(delegate(MapRegion R) { R.Color = Cardamom.Utilities.ColorMath.MakeColor(R.Region.Culture.Toughness, R.Region.Culture.Individualism, R.Region.Culture.PowerDistance); });
            //_Map.ChangeViewRanged(delegate(MapRegion Region) { return (float)Region.Region.FPC; }, Color.White, Color.Red);
            //_Map.ChangeViewRanged(delegate(MapRegion Region) { return (float)_Map.GetRegionAt(E.Position).Region.Desirability(Region.Region); }, Color.White, Color.Red);
        }

        private void RightClickedMap(object Sender, MouseEventArgs E)
        {
            _Map.ChangeView(delegate(MapRegion R) { R.BiomeColor(); });
        }
    }
}
