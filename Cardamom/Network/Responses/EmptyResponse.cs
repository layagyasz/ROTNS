using System;

using Cardamom.Serialization;

namespace Cardamom.Network.Responses
{
	public class EmptyResponse : RPCResponse
	{
		public EmptyResponse() { }

		public EmptyResponse(SerializationInputStream Stream)
			: base(Stream) { }
	}
}
