using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public interface TranscendentalFilter
    {
        void Filter(FloatingImage Image, FloatingImage Field);
    }
}
