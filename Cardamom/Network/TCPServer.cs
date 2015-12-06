using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
        MessageFactory _Factory;
        ushort _Port;
        IPAddress _Address;

        public ushort Port { get { return _Port; } }
        public IPAddress Address { get { return _Address; } }

        public TCPServer(ushort Port, MessageFactory Factory)
        {
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Port = Port;
            _Factory = Factory;

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

        private void Received(object Sender, MessageReceivedEventArgs E)
        {
            if (OnMessageReceived != null) OnMessageReceived(Sender, E);
        }

        private void HandleDrop(object Sender, EventArgs E)
        {
            if (OnConnectionLost != null) OnConnectionLost(Sender, E);
        }

        private void Listen()
        {
            IPEndPoint EndPoint = new IPEndPoint(_Address, _Port);

            _Socket.Bind(EndPoint);
            _Socket.Listen(10);

            while (_Socket != null)
            {
                try
                {
                    Socket Incoming = _Socket.Accept();
                    TCPConnection Connection = new TCPConnection(Incoming, _Factory);
                    Connection.OnMessageReceived += new TCPConnection.MessageReceivedEventHandler(Received);
                    Connection.Start();
                    Connection.OnConnectionLost += new EventHandler(HandleDrop);
                    if (OnConnectionReceived != null) OnConnectionReceived(Connection, EventArgs.Empty);
                }
                catch (Exception e) { Console.WriteLine(e); }
            }
        }
    }
}
