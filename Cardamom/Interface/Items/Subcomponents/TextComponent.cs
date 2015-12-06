using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;

using Cardamom.Planar;

namespace Cardamom.Interface.Items.Subcomponents
{
    class TextComponent
    {
        Text _Text;

        public string String { get { return _Text.DisplayedString; } set { _Text.DisplayedString = value; } }
        public Font Font { get { return _Text.Font; } set { _Text.Font = value; } }
        public Color Color { get { return _Text.Color; } set { _Text.Color = value; } }

        public TextComponent(string ClassName)
        {
            _Text = new Text();
            _Text.Font = (Font)ClassLibrary.Instance[ClassName]["font"];
            _Text.Color = (Color)ClassLibrary.Instance[ClassName]["fontcolor"];
        }

        public void Draw(RenderTarget Target, PlanarTransformMatrix Transform)
        {
            _Text.Position = Transform * new Vector2f(1, 1);
            Target.Draw(_Text);
        }
    }
}
