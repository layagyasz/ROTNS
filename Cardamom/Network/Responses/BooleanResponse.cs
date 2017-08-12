using System;

using Cardamom.Serialization;

namespace Cardamom.Network.Responses
{
	public class BooleanResponse : RPCResponse
	{
		public readonly bool Value;

		public BooleanResponse(bool Value)
		{
			this.Value = Value;
		}

		public BooleanResponse(SerializationInputStream Stream)
			: base(Stream)
		{
			Value = Stream.ReadBoolean();
		}
	}
}
