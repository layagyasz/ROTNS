using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROTNS
{
    class Training
    {
        static readonly Training _Default = new Training();
        public static Training Default { get { return _Default; } }

        float _Endurance = 1;
        float _Speed = 1;
        float _RangeAccuracy = .65f;
        float _MeleeAccuracy = .8f;
        float _ArtilleryAccuracy = .5f;
        float _Morale = 1;
        float _Discipline = 1;

        public float Endurance { get { return _Endurance; } }
        public float Speed { get { return _Speed; } }
        public float RangeAccuracy { get { return _RangeAccuracy; } }
        public float MeleeAccuracy { get { return _MeleeAccuracy; } }
        public float ArtilleryAccuracy { get { return _ArtilleryAccuracy; } }
        public float Morale { get { return _Morale; } }
        public float Discipline { get { return _Discipline; } }
    }
}
