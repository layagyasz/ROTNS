using System;

using Cardamom.Serialization;

namespace Cardamom.Network
{
	public abstract class RPCRequest : Serializable
	{
		public int Id;

		public RPCRequest() { }

		public RPCRequest(SerializationInputStream Stream)
		{
			Id = Stream.ReadInt32();
		}

		public virtual void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Id);
		}
	}
}
