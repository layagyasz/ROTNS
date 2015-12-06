using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;

using Cardamom.Planar;

namespace Cardamom.Interface
{
    public abstract class ClassedGuiItem : Interactive
    {
        protected Class _Class;
        protected Class.Mode _Mode;
        protected Class.Mode _LastMode;
        protected Class.Mode _ThirdMode;

        private Dictionary<string, float> _Transitions;
        private bool _Transitioning;

        protected bool Transitioning { get { return _Transitioning; } }

        public Class.Mode Mode
        {
            get { return _Mode; }
            set
            {
                _Transitioning = true;
                _ThirdMode = _LastMode;
                _LastMode = _Mode;
                _Mode = value;
                ScaleTransitions();
            }
        }

        public ClassedGuiItem()
            : base()
        {
            this.Clicked += new EventHandler<MouseEventArgs>(HandleClick);
            this.MouseOut += new EventHandler<MouseEventArgs>(HandleMouseOut);
            this.MouseOver += new EventHandler<MouseEventArgs>(HandleMouseOver);
            this.Leave += new EventHandler<MouseEventArgs>(HandleLeave);
        }

        public ClassedGuiItem(string ClassName)
            : this()
        {
            _Class = ClassLibrary.Instance[ClassName];
            _Transitions = new Dictionary<string, float>();
            IEnumerator<string> I = _Class.IncrementedAttributes;
            while (I.MoveNext())
            {
                _Transitions.Add(I.Current, 1);
            }
            Mode = Class.Mode.None;
            Mode = Class.Mode.None;
        }

        private void HandleMouseOver(object Sender, EventArgs E)
        {
            if (Mode == Class.Mode.None) Mode = Class.Mode.Hover;
        }

        private void HandleMouseOut(object Sender, EventArgs E)
        {
            if (Mode == Class.Mode.Hover) Mode = Class.Mode.None;
        }

        private void HandleClick(object Sender, EventArgs E)
        {
            Mode = Class.Mode.Focus;
        }

        private void HandleLeave(object Sender, EventArgs E)
        {
            Mode = Class.Mode.None;
        }

        private void IncrementTransitions(int DeltaT)
        {
            bool c = false;
            IEnumerator<string> I = _Class.IncrementedAttributes;
            while (I.MoveNext())
            {
                int t = (int)_Class[_Mode][I.Current];
                float n = _Transitions[I.Current] + (float)DeltaT / t;
                if (n >= 1) n = 1;
                else c = true;
                _Transitions[I.Current] = n;
            }
            _Transitioning = c;
        }

        private void ScaleTransitions()
        {
            if (_ThirdMode == _Mode)
            {
                IEnumerator<string> I = _Class.IncrementedAttributes;
                while (I.MoveNext())
                {
                    _Transitions[I.Current] = 1 - _Transitions[I.Current];
                }
            }
            else
            {
                IEnumerator<string> I = _Class.IncrementedAttributes;
                while (I.MoveNext())
                {
                    _Transitions[I.Current] = 0;
                }
            }
        }

        public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, PlanarTransformMatrix Transform)
        {
            base.Update(MouseController, KeyController, DeltaT, Transform);

            if (_Transitioning)
            {
                IncrementTransitions(DeltaT);
                PerformTransitions(_Transitions);
            }
        }

        public abstract void PerformTransitions(Dictionary<string, float> Transitions);
    }
}
