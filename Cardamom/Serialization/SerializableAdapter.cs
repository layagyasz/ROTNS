using System;
using System.IO;

using Cardamom.Network;

namespace Cardamom.Serialization
{
	public abstract class SerializableAdapter
	{
		Type[] _Messages;
		Func<SerializationInputStream, Serializable>[] _Deserializers;

		public SerializableAdapter(Tuple<Type, Func<SerializationInputStream, Serializable>>[] Messages)
		{
			_Messages = new Type[Messages.Length];
			_Deserializers = new Func<SerializationInputStream, Serializable>[Messages.Length];

			for (int i = 0; i < Messages.Length; ++i)
			{
				_Messages[i] = Messages[i].Item1;
				_Deserializers[i] = Messages[i].Item2;
			}
		}

		public void Send(Serializable Message, TCPConnection Connection)
		{
			SerializationOutputStream stream = new SerializationOutputStream(new MemoryStream());
			Serialize(Message, stream);
			Connection.Send(stream);
		}

		public void Serialize(Serializable Message, SerializationOutputStream Stream)
		{
			Stream.Write((byte)Array.IndexOf(_Messages, Message.GetType()));
			Stream.Write(Message);
		}

		public Serializable Deserialize(SerializationInputStream Stream)
		{
			return _Deserializers[Stream.ReadByte()](Stream);
		}
	}
}
