using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Cardamom.Serialization
{
	public class SerializationOutputStream : SerializationStream
	{
		public SerializationOutputStream(Stream Stream)
		{
			_Stream = Stream;
		}

		public void Write(SerializationOutputStream Stream)
		{
			Write(Stream.Length);
			using (MemoryStream M = new MemoryStream())
			{
				Stream._Stream.Seek(0, SeekOrigin.Begin);
				Stream._Stream.CopyTo(M);
				Stream._Stream.Seek(0, SeekOrigin.End);
				foreach (byte b in M.ToArray())
				{
					Write(b);
				}
			}
		}

		public void Write(string String)
		{
			if (String == null) Write((int)0);
			else
			{
				Write(String.Length);
				foreach (char C in String) Write(C);
			}
		}

		public void Write(bool Bool)
		{
			_Stream.Write(BitConverter.GetBytes(Bool), 0, 1);
		}

		public void Write(byte Byte)
		{
			_Stream.WriteByte(Byte);
		}

		public void Write(ushort UInt16)
		{
			_Stream.Write(BitConverter.GetBytes(UInt16), 0, 2);
		}

		public void Write(short Int16)
		{
			_Stream.Write(BitConverter.GetBytes(Int16), 0, 2);
		}

		public void Write(uint UInt32)
		{
			_Stream.Write(BitConverter.GetBytes(UInt32), 0, 4);
		}

		public void Write(int Int32)
		{
			_Stream.Write(BitConverter.GetBytes(Int32), 0, 4);
		}

		public void Write(long Int64)
		{
			_Stream.Write(BitConverter.GetBytes(Int64), 0, 8);
		}

		public void Write(float Float)
		{
			_Stream.Write(BitConverter.GetBytes(Float), 0, 4);
		}

		public void Write(double Double)
		{
			_Stream.Write(BitConverter.GetBytes(Double), 0, 8);
		}

		public void Write(Serializable Serializable)
		{
			Serializable.Serialize(this);
		}

		public void Write<T>(T[] Array, Action<T> Serializer)
		{
			Write(Array.Length);
			foreach (T S in Array) Serializer(S);
		}

		public void Write<T>(T[] Array) where T : Serializable
		{
			Write(Array.Length);
			foreach (Serializable S in Array) S.Serialize(this);
		}

		public void Write<T>(IEnumerable<T> List) where T : Serializable
		{
			Write(List.Count());
			foreach (Serializable S in List) S.Serialize(this);
		}
	}
}
