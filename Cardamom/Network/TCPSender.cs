using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using Cardamom.Serialization;

namespace Cardamom.Network
{
	class TCPSender
	{
		public event EventHandler OnConnectionLost;

		Socket _Socket;
		List<Message> _MessageQueue;
		Thread _SenderThread;
		SerializationOutputStream _Stream;
		RPCAdapter _Adapter;

		public TCPSender(Socket Socket, RPCAdapter Adapter)
		{
			_Socket = Socket;
			_Stream = new SerializationOutputStream(new NetworkStream(_Socket));
			_Adapter = Adapter;
			_MessageQueue = new List<Message>();
		}

		public void Start()
		{
			_SenderThread = new Thread(new ThreadStart(SenderThread));
			_SenderThread.Start();
		}

		public void Stop()
		{
			_Socket = null;
			_Stream.Close();
		}

		public void Send(Message Message)
		{
			lock (_MessageQueue)
			{
				_MessageQueue.Add(Message);
				Monitor.Pulse(_MessageQueue);
			}
		}

		private void SenderThread()
		{
			while (_MessageQueue != null && _Socket != null)
			{
				Message s = null;

				lock (_MessageQueue)
				{
					if (_MessageQueue.Count > 0)
					{
						s = _MessageQueue[0];
						_MessageQueue.RemoveAt(0);
					}
					else Monitor.Wait(_MessageQueue);
				}
				if (s != null)
				{
					try
					{
						SerializationOutputStream stream = new SerializationOutputStream(new MemoryStream());
						_Adapter.Serialize(s, stream);
						_Stream.Write(stream);
						_Stream.Flush();
					}
					catch (Exception e)
					{
						if (OnConnectionLost != null) OnConnectionLost(this, EventArgs.Empty);
						_Socket = null;
					}
				}
			}
		}
	}
}
