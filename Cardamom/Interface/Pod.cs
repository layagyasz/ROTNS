using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;

using Cardamom.Planar;

namespace Cardamom.Interface
{
    public interface Pod : Cardamom.Serialization.XmlReadable
    {
        void Update(MouseController MouseController, KeyController KeyController, int DeltaT, PlanarTransformMatrix Transform);
        void Draw(RenderTarget Target, PlanarTransformMatrix Transform);
    }
}
