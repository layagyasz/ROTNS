using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndrassyII
{
    public class Generated<T> : IEnumerable<T>
    {
        protected List<T> _Sounds = new List<T>();

        T Last { get { return _Sounds[_Sounds.Count - 1]; } }
        T First { get { return _Sounds[0]; } }
        public int Length { get { return _Sounds.Count; } }

        public IEnumerator<T> GetEnumerator() { return _Sounds.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int Index] { get { return _Sounds[Index]; } }

        public Generated() { }
        public Generated(T Initial) { _Sounds.Add(Initial); }


        public void Combine(Generated<T> Word)
        {
            if(Word != null) foreach (T S in Word._Sounds) _Sounds.Add(S);
        }

        public void Remove(int Index, int Length)
        {
            _Sounds.RemoveRange(Index, Length);
        }

        public void Insert(int Index, List<T> Values)
        {
            for (int i = Values.Count - 1; i >= 0; --i)
            {
                _Sounds.Insert(Index, Values[i]);
            }
        }

        public void RemoveDoubles()
        {
            for (int i = 0; i < _Sounds.Count - 1; ++i)
            {
                if (_Sounds[i].Equals(_Sounds[i + 1])) _Sounds.RemoveAt(i);
            }
        }
    }
}
