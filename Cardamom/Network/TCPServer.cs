using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Cardamom.Serialization;
using System.IO;

namespace Cardamom.Network
{
	public class TCPServer
	{
		public delegate void MessageReceivedEventHandler(object Sender, MessageReceivedEventArgs E);
		public event MessageReceivedEventHandler OnMessageReceived;
		public event EventHandler OnConnectionLost;

		public event EventHandler OnConnectionReceived;

		Socket _Socket;
		Thread _ServerThread;
		ushort _Port;
		IPAddress _Address;
		List<TCPConnection> _Connections = new List<TCPConnection>();

		public SerializableAdapter MessageAdapter;
		public RPCHandler RPCHandler;

		public ushort Port { get { return _Port; } }
		public IPAddress Address { get { return _Address; } }

		public TCPServer(ushort Port)
		{
			_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_Port = Port;

			IPHostEntry Host = Dns.Resolve(Dns.GetHostName());
			_Address = Host.AddressList[0];
		}

		public void Start()
		{
			_ServerThread = new Thread(new ThreadStart(Listen));
			_ServerThread.Start();
		}

		public void Close()
		{
			Socket S = _Socket;
			_Socket = null;
			S.Close();
		}

		public void Broadcast(RPCRequest Request)
		{
			if (MessageAdapter != null && RPCHandler != null)
			{
				lock (_Connections)
				{
					_Connections.ForEach(i => RPCHandler.Call(Request, MessageAdapter, i));
				}
			}
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

		private void HandleDrop(object Sender, EventArgs E)
		{
			if (OnConnectionLost != null) OnConnectionLost(Sender, E);
		}

		private void Listen()
		{
			IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, _Port);

			_Socket.Bind(EndPoint);
			_Socket.Listen(10);

			while (_Socket != null)
			{
				try
				{
					Socket Incoming = _Socket.Accept();
					TCPConnection Connection = new TCPConnection(Incoming);
					Connection.OnMessageReceived += Received;
					Connection.Start();
					Connection.OnConnectionLost += HandleDrop;
					lock (_Connections)
					{
						_Connections.Add(Connection);
					}
					if (OnConnectionReceived != null) OnConnectionReceived(Connection, EventArgs.Empty);
				}
				catch (Exception e) { Console.WriteLine(e); }
			}
		}
	}
}
