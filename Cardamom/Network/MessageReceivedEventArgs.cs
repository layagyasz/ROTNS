using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Cardamom.Network
{
    public class MessageReceivedEventArgs
    {
        Message _Message;

        public Message Message { get { return _Message; } }

        public MessageReceivedEventArgs(Message Message)
        {
            _Message = Message;
        }
    }
}
