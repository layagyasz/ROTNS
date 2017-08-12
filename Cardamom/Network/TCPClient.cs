using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

using Cardamom.Serialization;
using System.IO;

namespace Cardamom.Network
{
	public class TCPClient
	{
		public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;
		public event EventHandler OnConnectionLost;

		TCPConnection _Connection;

		public SerializableAdapter MessageAdapter;
		public RPCHandler RPCHandler;

		public TCPClient(string IP, ushort Port)
		{
			Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Socket.Connect(IPAddress.Parse(IP), Port);

			_Connection = new TCPConnection(Socket);
			_Connection.OnMessageReceived += Received;
			_Connection.OnConnectionLost += HandleDrop;
		}

		private void HandleDrop(object Sender, EventArgs E)
		{
			if (OnConnectionLost != null) OnConnectionLost(Sender, E);
		}

		public void Start()
		{
			_Connection.Start();
		}

		public void Close()
		{
			_Connection.Close();
		}

		public void Send(SerializationOutputStream Message) { _Connection.Send(Message); }

		public Promise Call(RPCRequest Request)
		{
			if (RPCHandler != null && MessageAdapter != null)
				return RPCHandler.Call(Request, MessageAdapter, _Connection);
			else throw new InvalidOperationException("No RPC or Message Adapter provided.");
		}

		private void Received(object Sender, MessageReceivedEventArgs E)
		{
			if (MessageAdapter != null && RPCHandler != null)
			{
				Serializable m = MessageAdapter.Deserialize(new SerializationInputStream(new MemoryStream(E.Message)));
				RPCHandler.HandleMessage(m, MessageAdapter, (TCPConnection)Sender);
			}

			if (OnMessageReceived != null) OnMessageReceived(Sender, E);
		}
	}
}
