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
    class TCPReceiver
    {
        public delegate void MessageReceivedEventHandler(object Sender, MessageReceivedEventArgs E);
        public event MessageReceivedEventHandler OnMessageReceived;
        public event EventHandler OnConnectionLost;

        Socket _Socket;
        NetworkStream _Stream;
        Thread _ReceiverThread;
		RPCAdapter _Adapter;

        public TCPReceiver(Socket Socket, RPCAdapter Adapter, TCPConnection Connection)
        {
            _Socket = Socket;
            _Stream = new NetworkStream(_Socket);
			_Adapter = Adapter;
        }

        public void Start()
        {
            _ReceiverThread = new Thread(new ThreadStart(ReceiverThread));
            _ReceiverThread.Start();
        }

        public void Stop()
        {
            _Socket = null;
            _Stream.Close();
        }

        private byte[] Receive()
        {
            try
            {
                long length;
                byte[] b = null;
                if (_Socket != null)
                {
                    byte[] i = new byte[8];
                    length = _Stream.Read(i, 0, 8);
                    length = BitConverter.ToInt64(i, 0);
                    b = new byte[length];
                    int atByte = 0;
                    do
                    {
                        atByte += _Stream.Read(b, atByte, (int)length - atByte);
                    }
                    while (atByte < length);
                }
                return b;
            }
            catch (Exception e) { return null; }
        }

        private void ReceiverThread()
        {
            while (_Socket != null)
            {
                byte[] data = Receive();
                if (data != null)
                {
                    SerializationInputStream S = new SerializationInputStream(new MemoryStream(data));
                    if (OnMessageReceived != null) OnMessageReceived(this, new MessageReceivedEventArgs(S));
                }
                else
                {
                    if (OnConnectionLost != null) OnConnectionLost(this, EventArgs.Empty);
					_Socket = null;
                }
            }
        }
    }
}
