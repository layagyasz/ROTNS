using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace Cardamom.Network
{
    public class MessageFactory
    {
        Dictionary<Type, byte> _MessageTypes;
        Func<SerializationInputStream, Message>[] _Constructors;

        public MessageFactory()
        {
            _MessageTypes = new Dictionary<Type, byte>();
            _Constructors = new Func<SerializationInputStream, Message>[256];
        }

        public Message Deserialize(SerializationInputStream Stream)
        {
            return _Constructors[Stream.ReadByte()].Invoke(Stream);
        }

        public void AddMessageType(Type Type, byte ID, Func<SerializationInputStream, Message> Constructor) 
        { 
            _MessageTypes.Add(Type, ID);
            _Constructors[ID] = Constructor;
        }

        public void Serialize(Message Message, SerializationOutputStream Stream)
        {
            Stream.Write(_MessageTypes[Message.GetType()]);
            Stream.Write(Message);
        }
    }
}
