using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace Cardamom.Network
{
	public class TCPConnection
	{
		public delegate void MessageReceivedEventHandler(object Sender, MessageReceivedEventArgs E);
		public event MessageReceivedEventHandler OnMessageReceived;
		public event EventHandler OnConnectionLost;

		Socket _Socket;
		TCPReceiver _Receiver;
		TCPSender _Sender;

		bool _Connected;
		public bool Connected { get { return _Connected; } }

		public TCPConnection(Socket Socket)
		{
			_Socket = Socket;
			_Receiver = new TCPReceiver(_Socket);
			_Sender = new TCPSender(_Socket);
		}

		public void Start()
		{
			_Connected = true;
			_Receiver.Start();
			_Sender.Start();
			_Receiver.OnMessageReceived += Received;
			_Receiver.OnConnectionLost += HandleDrop;
			_Sender.OnConnectionLost += HandleDrop;
		}

		public void Close()
		{
			_Receiver.Stop();
			_Sender.Stop();
			_Socket.Close();
		}

		private void Received(object Sender, MessageReceivedEventArgs E)
		{
			if (OnMessageReceived != null) OnMessageReceived(this, E);
		}

		private void HandleDrop(object Sender, EventArgs E)
		{
			_Connected = false;
			if (OnConnectionLost != null) OnConnectionLost(this, E);
		}

		public void Send(SerializationOutputStream Message)
		{
			_Sender.Send(Message);
		}

		public override string ToString()
		{
			return "[TCPConnection]" + _Socket.LocalEndPoint.ToString();
		}
	}
}
