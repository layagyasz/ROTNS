using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

using ROTNS.Model;

namespace ROTNS.View
{
    public class WorldView : Interactive
    {
        World _World;
        RenderTexture _RenderTarget;
        bool _Redraw = true;
        Vertex[] _Corners;
        Vertex[] _Vertices;
        Dictionary<Region, RegionView> _Regions = new Dictionary<Region, RegionView>();
		Container<GuiItem> _Overlay = new Container<GuiItem>();

        public override Vector2f Size { get { return new Vector2f(_World.Width, _World.Height); } }

        public World World { get { return _World; } }

        public WorldView(World World, Vector2f DrawSize)
        {
            _World = World;
            _RenderTarget = new RenderTexture((uint)_World.Settings.Width, (uint)_World.Settings.Height);

            _Vertices = new Vertex[World.Settings.Width * World.Settings.Height];
            for (int i = 0; i < World.Settings.Width; ++i)
            {
                for (int j = 0; j < World.Settings.Height; ++j)
                {
                    float h = World.HeightAt(i, j);
                    float m = World.Moisture(i, j);
                    float t = World.TemperatureAt(i, j);
                    _Vertices[i * World.Settings.Height + j].Position = new Vector2f(i, j);
                    _Vertices[i * World.Settings.Height + j].Color = Color.White;
                }
            }

            foreach (Region R in _World.Regions)
            {
                RegionView M = new RegionView(this, R);
                _Regions.Add(R, M);
            }

            _Corners = new Vertex[]
            {
                new Vertex(new Vector2f(0,0), new Vector2f(0,0)), 
                new Vertex(new Vector2f(DrawSize.X, 0), new Vector2f(_World.Settings.Width - 1, 0)),
                new Vertex(new Vector2f(DrawSize.X, DrawSize.Y), new Vector2f(_World.Settings.Width - 1, _World.Settings.Height - 1)),
                new Vertex(new Vector2f(0, DrawSize.Y), new Vector2f(0, _World.Settings.Height - 1))
            };
        }

        public RegionView GetViewFor(Region Region) { return _Regions[Region]; }

        public void HighlightRegion(RegionView Region, bool Highlighted)
        {
            Region.SetHighlight(Highlighted);
            _Redraw = true;
        }

        public void ChangeView(Action<RegionView> View)
        {
            foreach (RegionView R in _Regions.Values) R.SetView(View);
            _Redraw = true;
        }

        public void ChangeView(Func<RegionView, float> View, Color Color1, Color Color2)
        {
            float Max = float.MinValue;
            float Min = float.MaxValue;

            foreach (RegionView R in _Regions.Values)
            {
                float v = View.Invoke(R);
                if (v > Max && !double.IsInfinity(v)) Max = v;
                if (v < Min && !double.IsInfinity(v)) Min = v;
            }
            foreach (RegionView R in _Regions.Values)
            {
                float v = View.Invoke(R);
                v = (v - Min) / (Max - Min);
                if (!float.IsInfinity(v) || !float.IsNaN(v)) R.SetView(i => i.SetColor(Cardamom.Utilities.ColorMath.BlendColors(Color1, Color2, v)));
                else R.SetView(i => i.SetColor(new Color(128, 128, 128)));
            }
            _Redraw = true;
        }

        public RegionView GetRegionAt(Vector2f Position)
        {
            int X = (int)((Position.X - this.Position.X) / _Corners[2].Position.X * _World.Settings.Width);
            int Y = (int)((Position.Y - this.Position.Y) / _Corners[2].Position.Y * _World.Settings.Height);
            return GetViewFor(_World.Get(X, Y).Region);
        }

        public void MultiplyColor(int X, int Y, Color Color)
        {
            _Vertices[X * _World.Settings.Height + Y].Color = Cardamom.Utilities.ColorMath.Multiply(_Vertices[X * _World.Settings.Height + Y].Color, Color);
        }
        public void SetColor(int X, int Y, Color Color)
        {
            double s = _World.Shade(X, Y) * .5f + .5f;
            _Vertices[X * _World.Settings.Height + Y].Color = new Color((byte)(Math.Min(Color.R * s, 255)), (byte)(Math.Min(Color.G * s, 255)), (byte)(Math.Min(Color.B * s, 255)));
        }
        public Color GetColor(int X, int Y) { return _Vertices[X * _World.Settings.Height + Y].Color; }

		public void AddOverlay(GuiItem Overlay) { _Overlay.Add(Overlay); }

		public void RemoveOverlay(GuiItem Overlay) { _Overlay.Remove(Overlay); }

        public override bool IsCollision(Vector2f Point)
        {
            return Point.X > 0 && Point.X < _Corners[2].Position.X &&
                Point.Y > 0 && Point.Y < _Corners[2].Position.Y;
        }

        public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
        {
            Transform.Translate(Position);
            base.Update(MouseController, KeyController, DeltaT, Transform);
			_Overlay.Update(MouseController, KeyController, DeltaT, Transform);
        }

        RenderStates _RenderState;
        public override void Draw(RenderTarget Target, Transform Transform)
        {
            Transform.Translate(Position);
            _RenderState.Transform = Transform;
            if (_Redraw)
            {
                _RenderTarget.Draw(_Vertices, PrimitiveType.Points);
                _RenderTarget.Display();
                _RenderState = new RenderStates(_RenderTarget.Texture);
                _Redraw = false;
            }
            Target.Draw(_Corners, PrimitiveType.Quads, _RenderState);
			_Overlay.Draw(Target, Transform);
        }
    }
}
