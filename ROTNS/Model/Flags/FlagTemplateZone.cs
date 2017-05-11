using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;
using Cardamom.Graphing;

using SFML.Graphics;
using SFML.Window;

namespace ROTNS.Model.Flags
{
    class FlagTemplateZone : GraphNode<string>
    {
        int _ID;
        Vector2f[] _Vertices;

        public int ID { get { return _ID; } }
        public Vector2f[] Vertices { get { return _Vertices; } }

        public FlagTemplateZone(string Value, int ID)
            : base(Value) { _ID = ID; }

        public void Initialize(ParseBlock Block, FlagTemplate Template, TextureSheet Textures)
        {
            foreach (ParseBlock B in Block.Break())
            {
                switch (B.Name.Trim().ToLower())
                {
                    case "image": Textures.AddImage(this.Value, new SFML.Graphics.Image(B.String)); break;
                    case "edge":
                        string[] data = B.String.Split(' ');
                        this.AddNeighbor((GraphNode<string>)Template.GetNode(data[0]), Convert.ToSingle(data[1], System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    case "vertices": _Vertices = Cardamom.Interface.ClassLibrary.Instance.ParseVector2fs(B.String); break;
                }
            }
        }


    }
}
