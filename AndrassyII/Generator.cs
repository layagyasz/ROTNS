using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndrassyII
{
    interface Generator<T>
    {
        double Frequency { get; }
        Generated<T> Generate(Random Random);
    }
}
