using System;
using System.Threading;

namespace Cardamom.Network
{
	public class Promise
	{
		object _lock = new object();

		RPCResponse _Response;
		bool _Satisfied;

		public RPCResponse Get()
		{
			lock (_lock)
			{
				if (!_Satisfied) Monitor.Wait(_lock);
				return _Response;
			}
		}

		public void Set(RPCResponse Response)
		{
			lock (_lock)
			{
				_Response = Response;
				_Satisfied = true;
				Monitor.PulseAll(_lock);
			}
		}
	}
}
