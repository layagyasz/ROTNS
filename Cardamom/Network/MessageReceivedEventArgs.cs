using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Cardamom.Network
{
	public class MessageReceivedEventArgs
	{
		public readonly Message Message;

		public MessageReceivedEventArgs(Message Message)
		{
			this.Message = Message;
		}
	}
}
