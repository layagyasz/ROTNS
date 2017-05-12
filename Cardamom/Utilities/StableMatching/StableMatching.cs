using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.StableMatching
{
    public class StableMatching<T,K>
    {
        Dictionary<T, Actor<T, K>> _PrimaryActors = new Dictionary<T, Actor<T, K>>();
        Dictionary<K, Actor<K, T>> _SecondaryActors = new Dictionary<K, Actor<K, T>>();

        public void AddPrimaryActor(T Value)
        {
            Actor<T, K> Actor = new Actor<T,K>(Value);
            _PrimaryActors.Add(Value, Actor);
            foreach (KeyValuePair<K, Actor<K, T>> Secondary in _SecondaryActors) Secondary.Value.AddActor(Actor);
        }

        public void AddSecondaryActor(K Value)
        {
            Actor<K, T> Actor = new Actor<K, T>(Value);
            _SecondaryActors.Add(Value, Actor);
            foreach (KeyValuePair<T, Actor<T, K>> Secondary in _PrimaryActors)  Secondary.Value.AddActor(Actor);
        }

        private void Match()
        {
            foreach (KeyValuePair<T, Actor<T, K>> Actor in _PrimaryActors)
            {
                FindPair(Actor.Value);
            }
        }

        public void SetPrimaryPreference(T Value, K Target, double Preference)
        {
            _PrimaryActors[Value].SetPreference(_SecondaryActors[Target], Preference);
        }

        public void SetSecondaryPreference(K Value, T Target, double Preference)
        {
            _SecondaryActors[Value].SetPreference(_PrimaryActors[Target], Preference);
        }

        private void FindPair(Actor<T, K> Actor)
        {
            while (Actor.Pair == null)
            {
                Actor<K, T> Choice = Actor.NextChoice();
                if (Choice.AcceptProposal(Actor))
                {
                    if (Choice.Pair != null)
                    {
                        Choice.Pair.Pair = null;
                        FindPair(Choice.Pair);
                    }
                    Choice.Pair = Actor;
                    Actor.Pair = Choice;
                }
            }
        }

        public K GetPair(T Value) { return _PrimaryActors[Value].Pair.Value; }

        public List<Pair<T, K>> GetPairs()
        {
            Match();
            List<Pair<T, K>> R = new List<Pair<T, K>>();
            foreach (KeyValuePair<T, Actor<T, K>> P in _PrimaryActors)
            {
                R.Add(new Pair<T, K>(P.Key, GetPair(P.Key)));
            }
            return R;
        }

        public override string ToString()
        {
            string R = "";
            foreach(KeyValuePair<T, Actor<T, K>> A in _PrimaryActors)
            {
                R += A.Value.ToString() + "\n";
            }
            return R;
        }
    }
}
