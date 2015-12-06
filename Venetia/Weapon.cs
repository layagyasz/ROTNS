using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROTNS
{
    class Weapon
    {
        enum WeaponType { Ranged, Melee, Artillery };

        static readonly Weapon _Default = new Weapon();
        public static Weapon Default { get { return _Default; } }

        WeaponType _Type = WeaponType.Ranged;
        float _Range = 100;
        float _Damage = 1;

        float _RoundAOE = 0;
        float _RoundAOEDamage = 0;
        float _LinearAOE = 0;
        float _LinearAOEDamage = 0;
    }
}
