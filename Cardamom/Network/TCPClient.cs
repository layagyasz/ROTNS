using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

using Cardamom.Serialization;

namespace Cardamom.Network
{
    public class TCPClient
    {
        public delegate void MessageReceivedEventHandler(object Sender, MessageReceivedEventArgs E);
        public event MessageReceivedEventHandler OnMessageReceived;
        public event EventHandler OnConnectionLost;

        TCPConnection _Connection;

        public TCPClient(string IP, ushort Port, RPCAdapter Adapter)
        {
            Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(IPAddress.Parse(IP), Port);

            _Connection = new TCPConnection(Socket, Adapter);
            _Connection.OnMessageReceived += new TCPConnection.MessageReceivedEventHandler(Received);
            _Connection.OnConnectionLost += new EventHandler(HandleDrop);
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

        private void Received(object Sender, MessageReceivedEventArgs E)
        {
            if (OnMessageReceived != null) OnMessageReceived(Sender, E);
        }
    }
}
