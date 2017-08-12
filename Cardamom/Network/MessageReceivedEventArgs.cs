using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Cardamom.Network
{
	public class MessageReceivedEventArgs : EventArgs
	{
		public readonly byte[] Message;

		public MessageReceivedEventArgs(byte[] Message)
		{
			this.Message = Message;
		}
	}
}
