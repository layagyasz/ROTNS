using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Cardamom.Network
{
	public abstract class RPCAdapter
	{
		public abstract Type[] Messages { get; }
		public abstract Func<SerializationInputStream, Message>[] Deserializers { get; }

		public void Serialize(Message Message, SerializationOutputStream Stream)
		{
			Stream.Write((byte)Array.IndexOf(Messages, typeof(Message)));
			Stream.Write(Message);
		}

		public Message Deserialize(SerializationInputStream Stream)
		{
			return Deserializers[Stream.ReadByte()](Stream);
		}
	}
}
