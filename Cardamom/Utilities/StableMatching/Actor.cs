using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;

namespace Cardamom.Utilities.StableMatching
{
    class Actor<T, K> : IComparable
    {
        T _Value;
        List<Pair<Actor<K, T>, double>> _Preferences = new List<Pair<Actor<K, T>, double>>();
        Dictionary<Actor<K, T>, double> _Actors = new Dictionary<Actor<K, T>, double>();

        Actor<K, T> _Pair;
        IEnumerator<Pair<Actor<K, T>, double>> _Iterator;

        public T Value { get { return _Value; } set { _Value = value; } }
        public Actor<K, T> Pair { get { return _Pair; } set { _Pair = value; } }

        public Actor(T Value) { _Value = Value; }

        internal Actor<K, T> NextChoice()
        {
            if (_Iterator == null) _Iterator = _Preferences.GetEnumerator();
            _Iterator.MoveNext();
            return _Iterator.Current.First;
        }

        internal bool AcceptProposal(Actor<K ,T> Actor)
        {
            return _Pair == null || GetPreference(Actor) > GetPreference(_Pair);
        }

        internal void AddActor(Actor<K, T> Actor)
        {
            _Preferences.Add(new Pair<Actor<K, T>, double>(Actor, 0));
            _Preferences.Sort(delegate(Pair<Actor<K, T>, double> p1, Pair<Actor<K, T>, double> p2) { return p1.Second.CompareTo(p2.Second);});
            _Actors.Add(Actor, 0);
        }

        public double GetPreference(Actor<K, T> Actor)
        {
            return _Actors[Actor];
        }

        public void SetPreference(Actor<K, T> Actor, double Preference)
        {
            if (_Actors.ContainsKey(Actor))
            {
                _Actors[Actor] = Preference;
                _Preferences.Remove(_Preferences.Find(i => i.First == Actor));
                _Preferences.Add(new Pair<Actor<K, T>, double>(Actor, -Preference));
            }
            else
            {
                _Preferences.Add(new Pair<Actor<K, T>, double>(Actor, -Preference));
                _Actors.Add(Actor, Preference);
            }
            _Preferences.Sort(delegate(Pair<Actor<K, T>, double> p1, Pair<Actor<K, T>, double> p2) { return p1.Second.CompareTo(p2.Second); });
        }

        public int CompareTo(object Other)
        {
            if (Other is Actor<T, K>) return ((Actor<T, K>)Other).Value.Equals(_Value) ? 0 : 1;
            else return 1;
        }

        public override string ToString()
        {
            string R = _Value.ToString() + "\n";
            foreach(Pair<Actor<K,T>, double> P in _Preferences)
            {
                R += P.First.Value.ToString() + " : " + P.Second + "\n";
            }
            return R;
        }
    }
}
