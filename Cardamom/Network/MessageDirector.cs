using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Network
{
    class MessageDirector
    {
        public delegate void MessageReceivedEventHandler(object Sender, MessageReceivedEventArgs E);
        public event MessageReceivedEventHandler OnMessageReceived;

        Dictionary<int, NetworkedComponent> _Components = new Dictionary<int, NetworkedComponent>();

        public void RegisterComponent(int ID, NetworkedComponent Component)
        {
            _Components.Add(ID, Component);
        }

        public void HandleMessage(object Sender, MessageReceivedEventArgs E)
        {
            if (E.Message is DirectedMessage)
            {
                _Components[((DirectedMessage)E.Message).ComponentID].HandleMessage(E.Message);
            }
            else if (OnMessageReceived != null) OnMessageReceived.Invoke(Sender, E);
        }
    }
}
