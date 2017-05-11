using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Spatial
{
    class Model : IEnumerable<Face>
    {
        Face[] _Faces;

        public IEnumerator<Face> GetEnumerator() { return (IEnumerator<Face>)_Faces.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get { return _Faces.Length; } }
    }
}
