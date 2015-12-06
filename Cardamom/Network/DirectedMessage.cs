using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Cardamom.Network
{
    abstract class DirectedMessage : Message
    {
        int _ComponentID;

        public int ComponentID { get { return _ComponentID; } }

        public DirectedMessage(SerializationInputStream Stream)
        {
            _ComponentID = Stream.ReadInt32();
        }

        public void Serialize(SerializationOutputStream Stream)
        {
            Stream.Write(_ComponentID);
        }
    }
}
