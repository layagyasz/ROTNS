using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;

namespace Cardamom.Interface
{
    public class MouseEventArgs : EventArgs
    {
        Vector2f _Position;

        public Vector2f Position { get { return _Position; } }

        public MouseEventArgs(Vector2f Position) { _Position = Position; }
    }
}
