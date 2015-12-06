using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Serialization
{
    public interface Serializable
    {
        void Serialize(SerializationOutputStream Stream);
    }
}
