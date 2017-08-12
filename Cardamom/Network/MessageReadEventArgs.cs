using System;

using Cardamom.Serialization;

namespace Cardamom.Network
{
	public class MessageReadEventArgs : EventArgs
	{
		public readonly Serializable Message;

		public MessageReadEventArgs(Serializable Message)
		{
			this.Message = Message;
		}
	}
}
