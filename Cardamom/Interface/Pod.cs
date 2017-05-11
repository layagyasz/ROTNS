using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;

namespace Cardamom.Interface
{
    public interface Pod : Cardamom.Serialization.XmlReadable
    {
        void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform);
        void Draw(RenderTarget Target, Transform Transform);
    }
}
