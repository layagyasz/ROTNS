using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cardamom.Serialization
{
	public class SerializationInputStream : SerializationStream
	{
		public SerializationInputStream(Stream Stream)
		{
			_Stream = Stream;
		}

		public bool ReadBoolean()
		{
			return ReadByte() != 0;
		}

		public byte ReadByte()
		{
			return (byte)_Stream.ReadByte();
		}

		public uint ReadUInt32()
		{
			byte[] v = new byte[4];
			_Stream.Read(v, 0, 4);
			return BitConverter.ToUInt32(v, 0);
		}

		public int ReadInt32()
		{
			byte[] v = new byte[4];
			_Stream.Read(v, 0, 4);
			return BitConverter.ToInt32(v, 0);
		}

		public long ReadInt64()
		{
			byte[] v = new byte[8];
			_Stream.Read(v, 0, 8);
			return BitConverter.ToInt64(v, 0);
		}

		public float ReadFloat()
		{
			byte[] v = new byte[4];
			_Stream.Read(v, 0, 4);
			return BitConverter.ToSingle(v, 0);
		}

		public double ReadDouble()
		{
			byte[] v = new byte[8];
			_Stream.Read(v, 0, 8);
			return BitConverter.ToDouble(v, 0);
		}

		public string ReadString()
		{
			byte[] v = new byte[ReadInt32() * 2];
			_Stream.Read(v, 0, v.Length);
			char[] s = Encoding.Unicode.GetChars(v);
			return new String(s);
		}

		public T[] ReadArray<T>(Func<T> Deserializer)
		{
			int l = ReadInt32();
			T[] arr = new T[l];
			for (int i = 0; i < l; ++i) arr[i] = Deserializer.Invoke();
			return arr;
		}

		public T[] ReadArray<T>(Func<SerializationInputStream, T> Deserializer)
		{
			int l = ReadInt32();
			T[] arr = new T[l];
			for (int i = 0; i < l; ++i) arr[i] = Deserializer.Invoke(this);
			return arr;
		}

		public IEnumerable<T> ReadEnumerable<T>(Func<T> Deserializer)
		{
			int l = ReadInt32();
			for (int i = 0; i < l; ++i) yield return Deserializer.Invoke();
		}

		public IEnumerable<T> ReadEnumerable<T>(Func<SerializationInputStream, T> Deserializer)
		{
			int l = ReadInt32();
			for (int i = 0; i < l; ++i) yield return Deserializer.Invoke(this);
		}
	}
}
