using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cardamom.Serialization
{
    public abstract class SerializationStream
    {
        protected Stream _Stream;

        public long Length { get { return _Stream.Length; } }
        public void Close() { _Stream.Close(); }
        public void Flush() { _Stream.Flush(); }

        public override string ToString()
        {
            using(MemoryStream S = new MemoryStream())
            {
                string r = "";
                long i = _Stream.Position;
                _Stream.Seek(0, SeekOrigin.Begin);
                _Stream.CopyTo(S);
                _Stream.Position = i;
                byte[] b = S.ToArray();
                foreach (byte B in b) r += B;
                return r;
            }
        }
    }
}
