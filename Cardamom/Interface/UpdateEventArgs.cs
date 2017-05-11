using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

namespace Cardamom.Interface
{
    public class UpdateEventArgs
    {
        MouseController _MouseController;
        KeyController _KeyController;
        int _DeltaT;
        Transform _Transform;

        public MouseController MouseController { get { return _MouseController; } }
        public KeyController KeyController { get { return _KeyController; } }
        public int DeltaT { get { return _DeltaT; } }
        public Transform Transform { get { return _Transform; } }

        public UpdateEventArgs(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
        {
            _MouseController = MouseController;
            _KeyController = KeyController;
            _DeltaT = DeltaT;
            _Transform = Transform;
        }
    }
}
