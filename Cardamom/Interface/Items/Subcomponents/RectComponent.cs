using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

namespace Cardamom.Interface.Items.Subcomponents
{
	class RectComponent : Component
	{
		Vertex[] _Vertices;
		Vector2f _Position;
		Texture _Texture;

		public Vector2f Position { get { return _Position; } set { _Position = value; } }
		public Vector2f Size { get { return new Vector2f(_Vertices[2].Position.X, _Vertices[2].Position.Y); } }

		public RectComponent(Class Class)
		{
			int Height = (int)Class.GetAttributeWithDefault("height", (int)0);
			int Width = (int)Class.GetAttributeWithDefault("width", (int)0);
			Color[] _Colors = (Color[])Class.GetAttributeWithDefault("background-color", ClassLibrary.NullColors);
			_Texture = Class.GetAttributeWithDefault<Texture>("background-image", null);
			if (_Texture == null)
			{
				_Vertices = new Vertex[]
				{
					new Vertex(new Vector2f(0,0), _Colors[0]),
					new Vertex(new Vector2f(Width, 0), _Colors[1]),
					new Vertex(new Vector2f(Width, Height), _Colors[2]),
					new Vertex(new Vector2f(0, Height), _Colors[3])
				};
			}
			else
			{
				Vector2f[] TexCoords =
					Class.GetAttributeWithDefault<Vector2f[]>("background-image-coords", ClassLibrary.NullVectors);
				_Vertices = new Vertex[]
				{
					new Vertex(new Vector2f(0,0), _Colors[0], TexCoords[0]),
					new Vertex(new Vector2f(Width, 0), _Colors[1], TexCoords[1]),
					new Vertex(new Vector2f(Width, Height), _Colors[2], TexCoords[2]),
					new Vertex(new Vector2f(0, Height), _Colors[3], TexCoords[3])
				};
			}
		}

		public Rectangle GetBoundingBox()
		{
			return new Rectangle(_Vertices[0].Position, _Vertices[2].Position - _Vertices[0].Position);
		}

		public void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(_Position);
			RenderStates R = (_Texture == null) ? new RenderStates() : new RenderStates(_Texture);
			R.Transform = Transform;
			Target.Draw(_Vertices, PrimitiveType.Quads, R);
		}

		public void PerformTransitions(Dictionary<string, float> Transitions, SubClass From, SubClass To)
		{
			Color[] FromC = (Color[])From.GetAttributeWithDefault("background-color", ClassLibrary.NullColors);
			Color[] ToC = (Color[])To.GetAttributeWithDefault("background-color", ClassLibrary.NullColors);

			float T = To.GetAttributeWithDefault(Transitions, "transition-background-color", "transition", 1);
			_Texture = (Texture)To.GetAttributeWithDefault("background-image", null);
			if (_Texture != null)
			{
				Vector2f[] TexCoords = (Vector2f[])To.GetAttributeWithDefault("background-image-coords", ClassLibrary.NullArray);
				for (int i = 0; i < 4; ++i) _Vertices[i].TexCoords = TexCoords[i];
			}

			for (int i = 0; i < 4; ++i) _Vertices[i].Color = Cardamom.Utilities.ColorMath.BlendColors(FromC[i], ToC[i], T);
		}
	}
}
