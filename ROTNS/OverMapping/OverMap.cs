using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface;
using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

using ROTNS.Core;

namespace ROTNS.OverMapping
{
    public class OverMap : Interactive
    {
        World _World;
        RenderTexture _RenderTarget;
        bool _Redraw = true;
        Vertex[] _Corners;
        Vertex[] _Vertices;
        List<MapRegion> _Regions = new List<MapRegion>();

        public World World { get { return _World; } }

        public OverMap(World World, Vector2f DrawSize)
        {
            _World = World;
            World.Marker = this;
            _RenderTarget = new RenderTexture((uint)_World.Settings.Width, (uint)_World.Settings.Height);

            _Vertices = new Vertex[World.Settings.Width * World.Settings.Height];
            for (int i = 0; i < World.Settings.Width; ++i)
            {
                for (int j = 0; j < World.Settings.Height; ++j)
                {
                    float h = World.Height(i, j);
                    float m = World.Moisture(i, j);
                    float t = World.Temperature(i, j);
                    _Vertices[i * World.Settings.Height + j].Position = new Vector2f(i, j);
                    _Vertices[i * World.Settings.Height + j].Color = Color.White;
                }
            }

            foreach (Region R in _World.Regions)
            {
                MapRegion M = new MapRegion(this, R);
                R.Marker = M;
                M.Region = R;
                M.BiomeColor();
                M.DiscoverBorder();
                _Regions.Add(M);
            }

            _Corners = new Vertex[]
            {
                new Vertex(new Vector2f(0,0), new Vector2f(0,0)), 
                new Vertex(new Vector2f(DrawSize.X, 0), new Vector2f(_World.Settings.Width - 1, 0)),
                new Vertex(new Vector2f(DrawSize.X, DrawSize.Y), new Vector2f(_World.Settings.Width - 1, _World.Settings.Height - 1)),
                new Vertex(new Vector2f(0, DrawSize.Y), new Vector2f(0, _World.Settings.Height - 1))
            };
        }

        public void ChangeView(Action<MapRegion> View)
        {
            foreach (MapRegion R in _Regions) View.Invoke(R);
            _Redraw = true;
        }

        public void ChangeViewRanged(Func<MapRegion, float> View, Color Color1, Color Color2)
        {
            float Max = float.MinValue;
            float Min = float.MaxValue;

            foreach (MapRegion R in _Regions)
            {
                float v = View.Invoke(R);
                if (v > Max && !double.IsInfinity(v)) Max = v;
                if (v < Min && !double.IsInfinity(v)) Min = v;
            }
            Console.WriteLine("{0} {1}", Min, Max);
            foreach (MapRegion R in _Regions)
            {
                float v = View.Invoke(R);
                v = (v - Min) / (Max - Min);
                R.Color = Cardamom.Utilities.ColorMath.BlendColors(Color1, Color2, v);
            }
            _Redraw = true;
        }

        public MicroRegion GetRegionAt(Vector2f Position)
        {
            int X = (int)(Position.X / _Corners[2].Position.X * _World.Settings.Width);
            int Y = (int)(Position.Y / _Corners[2].Position.Y * _World.Settings.Height);
            return _World.Get(X, Y);
        }

        public void SetColor(int X, int Y, Color Color)
        {
            double s = _World.Shade(X,Y);
            _Vertices[X * _World.Settings.Height + Y].Color = new Color((byte)(Math.Min(Color.R * s, 255)), (byte)(Math.Min(Color.G * s, 255)), (byte)(Math.Min(Color.B * s, 255)));
        }
        public Color GetColor(int X, int Y) { return _Vertices[X * _World.Settings.Height + Y].Color; }

        public override bool IsCollision(Vector2f Point)
        {
            return Point.X > 0 && Point.X < _Corners[2].Position.X &&
                Point.Y > 0 && Point.Y < _Corners[2].Position.Y;
        }

        public override void Update(MouseController MouseController, KeyController KeyController, int Delta, PlanarTransformMatrix Transform)
        {
            base.Update(MouseController, KeyController, Delta, Transform);
        }

        RenderStates _RenderState;
        public override void Draw(RenderTarget Target, PlanarTransformMatrix Transform)
        {
            if (_Redraw)
            {
                _RenderTarget.Draw(_Vertices, PrimitiveType.Points);
                _RenderTarget.Display();
                _RenderState = new RenderStates(_RenderTarget.Texture);
                _Redraw = false;
            }
            Target.Draw(_Corners, PrimitiveType.Quads, _RenderState);
        }
    }
}
