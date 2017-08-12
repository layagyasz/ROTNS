using System;

using Cardamom.Serialization;

namespace Cardamom.Network
{
	public abstract class RPCResponse : Serializable
	{
		public int RequestId;

		public RPCResponse() { }

		public RPCResponse(SerializationInputStream Stream)
		{
			RequestId = Stream.ReadInt32();
		}

		public virtual void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(RequestId);
		}
	}
}
