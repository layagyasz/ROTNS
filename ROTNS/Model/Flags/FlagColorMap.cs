using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROTNS.Model.Flags
{
    public class FlagColorMap
    {
        FlagColor[] _Colors;

        public FlagColor[] Colors { get { return _Colors; } }

        public FlagColorMap(FlagColor[] Colors)
        {
            _Colors = Colors;
        }

        public FlagColor[] Closest(Culture Culture, int Number)
        {
            float[] Keys = new float[_Colors.Length];
            for (int i = 0; i < _Colors.Length; ++i) Keys[i] = _Colors[i].DistanceTo(Culture);
            Array.Sort(Keys, _Colors);
            FlagColor[] R = new FlagColor[Number];
            Array.Copy(_Colors, R, Number);
            return R;
        }
    }
}
