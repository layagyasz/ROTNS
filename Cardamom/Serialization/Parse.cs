using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Serialization
{
    class Parse
    {
        public static T[] Array<T>(string Code, System.Globalization.CultureInfo CultureInfo, Func<string, System.Globalization.CultureInfo, T> Converter)
        {
            string[] def = Code.Split(' ');
            T[] r = new T[def.Length];
            for (int i = 0; i < def.Length; ++i)
            {
                r[i] = Converter.Invoke(def[i], CultureInfo);
            }
            return r;
        }

        public static T[] Array<T>(string Code, Func<string, T> Converter)
        {
            string[] def = Code.Split(' ');
            T[] r = new T[def.Length];
            for (int i = 0; i < def.Length; ++i)
            {
                r[i] = Converter.Invoke(def[i]);
            }
            return r;
        }
    }
}
