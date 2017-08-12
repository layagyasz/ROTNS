using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Cardamom.Network
{
	public class RPCHandler
	{
		int _NextId;
		Dictionary<Type, Func<RPCRequest, RPCResponse>> _Rpcs;
		Dictionary<int, Promise> _Promises = new Dictionary<int, Promise>();

		public RPCHandler()
		{
			_Rpcs = new Dictionary<Type, Func<RPCRequest, RPCResponse>>();
		}

		public RPCHandler(IDictionary<Type, Func<RPCRequest, RPCResponse>> Rpcs)
		{
			_Rpcs = Rpcs.ToDictionary(i => i.Key, i => i.Value);
		}

		public void RegisterRPC(Type Type, Func<RPCRequest, RPCResponse> ResponseGenerator)
		{
			_Rpcs.Add(Type, ResponseGenerator);
		}

		public Promise Call(RPCRequest Request, SerializableAdapter Adapter, TCPConnection Connection)
		{
			Promise p = new Promise();
			_Promises.Add(_NextId, p);
			Request.Id = _NextId;
			_NextId++;
			Adapter.Send(Request, Connection);
			return p;
		}

		public void HandleMessage(Serializable Message, SerializableAdapter Adapter, TCPConnection Connection)
		{
			if (Message is RPCRequest) HandleRequest((RPCRequest)Message, Adapter, Connection);
			else if (Message is RPCResponse) HandleResponse((RPCResponse)Message);
		}

		void HandleRequest(RPCRequest Rpc, SerializableAdapter Adapter, TCPConnection Connection)
		{
			if (_Rpcs.ContainsKey(Rpc.GetType()))
			{
				RPCResponse response = _Rpcs[Rpc.GetType()](Rpc);
				response.RequestId = Rpc.Id;
				Adapter.Send(response, Connection);
			}
		}

		void HandleResponse(RPCResponse Rpc)
		{
			_Promises[Rpc.RequestId].Set(Rpc);
			_Promises.Remove(Rpc.RequestId);
		}
	}
}
