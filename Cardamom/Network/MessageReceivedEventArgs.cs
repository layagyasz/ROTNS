using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Cardamom.Network
{
    public class MessageReceivedEventArgs
    {
		SerializationInputStream _Stream;

        public SerializationInputStream Stream { get { return _Stream; } }

        public MessageReceivedEventArgs(SerializationInputStream Stream)
        {
            _Stream = Stream;
        }
    }
}
