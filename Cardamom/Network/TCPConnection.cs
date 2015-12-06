﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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

        List<Pair<Query, Action<Response>>> _Queries = new List<Pair<Query, Action<Response>>>();
        Response _Response;

        bool _Connected;
        public bool Connected { get { return _Connected; } }

        public TCPConnection(Socket Socket, MessageFactory Factory)
        {
            _Socket = Socket;
            _Receiver = new TCPReceiver(_Socket, Factory, this);
            _Sender = new TCPSender(_Socket, Factory);
        }

        public void Start()
        {
            _Connected = true;
            _Receiver.Start();
            _Sender.Start();
            _Receiver.OnMessageReceived += new TCPReceiver.MessageReceivedEventHandler(Received);
            _Receiver.OnConnectionLost += new EventHandler(HandleDrop);
            _Sender.OnConnectionLost += new EventHandler(HandleDrop);
        }

        public void Close()
        {
            _Receiver.Stop();
            _Sender.Stop();
            _Socket.Close();
        }

        private void Received(object Sender, MessageReceivedEventArgs E)
		{
            if (E.Message is Response)
            {
                Pair<Query, Action<Response>> Q = _Queries[0];
                _Response = (Response)E.Message;
                if (Q.Second == null) Monitor.Pulse(Q.First);
                else Q.Second.Invoke(_Response);
            }
            else if (OnMessageReceived != null) OnMessageReceived(this, E);
        }

        private void HandleDrop(object Sender, EventArgs E)
        {
            _Connected = false;
            if (OnConnectionLost != null) OnConnectionLost(this, E);
        }

        public void Send(Message Message)
        {
            _Sender.Send(Message);
        }

        public Response Query(Query Query)
        {
            _Queries.Add(new Pair<Query, Action<Response>>(Query, null));
            Monitor.Wait(Query);
            return _Response;
        }

        public void AsynchronousQuery(Query Query, Action<Response> Action)
        {
            _Queries.Add(new Pair<Query, Action<Response>>(Query, Action));
        }

        public override string ToString()
        {
            return "[TCPConnection]" + _Socket.LocalEndPoint.ToString();
        }
    }
}
