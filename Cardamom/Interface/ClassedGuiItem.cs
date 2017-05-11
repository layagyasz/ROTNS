using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

namespace Cardamom.Interface
{
    public abstract class ClassedGuiItem : Interactive
    {
        public enum Series { Standard, NoFocus, Selectable };

        protected Class _Class;
        protected Class.Mode _Mode;
        protected Class.Mode _LastMode;
        protected Class.Mode _ThirdMode;
        protected Series _Series;

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
        public Class Class { get { return _Class; } }

        public ClassedGuiItem(Series Series = Series.Standard)
            : base()
        {
            _Series = Series;
            this.OnMouseOut += HandleMouseOut;
            this.OnMouseOver += HandleMouseOver;
            if (Series != Series.NoFocus)
            {
                this.OnClick += HandleClick;
                this.OnLeave += HandleLeave;
            }
        }

        public ClassedGuiItem(Class Class, Series Series = Series.Standard)
            : this(Series)
        {
			_Class = Class;
            _Transitions = new Dictionary<string, float>();
			foreach (string Transition in _Class.IncrementedAttributes)
            {
                _Transitions.Add(Transition, 1);
            }
            Mode = Class.Mode.None;
            Mode = Class.Mode.None;
            Mode = Class.Mode.None;
        }

        private void HandleMouseOver(object Sender, EventArgs E)
        {
            if (Mode == Class.Mode.None) Mode = Class.Mode.Hover;
            else if (Mode == Class.Mode.Selected) Mode = Class.Mode.SelectedHover;
        }

        private void HandleMouseOut(object Sender, EventArgs E)
        {
            if (Mode == Class.Mode.Hover) Mode = Class.Mode.None;
            else if (Mode == Class.Mode.SelectedHover) Mode = Class.Mode.Selected;
        }

        private void HandleClick(object Sender, EventArgs E)
        {
            if (_Series == Series.Selectable) Mode = Class.Mode.SelectedHover;
            else Mode = Class.Mode.Focus;
        }

        private void HandleLeave(object Sender, EventArgs E)
        {
            if (_Series != Series.Selectable) Mode = Class.Mode.None;
        }

        private void IncrementTransitions(int DeltaT)
        {
            bool c = false;
			foreach (string Transition in _Class.IncrementedAttributes)
            {
                int t = (int)_Class[_Mode][Transition];
                float n = _Transitions[Transition] + (float)DeltaT / t;
                if (n >= 1) n = 1;
                else c = true;
                _Transitions[Transition] = n;
            }
            _Transitioning = c;
        }

        private void ScaleTransitions()
        {
            if (_ThirdMode == _Mode)
            {
                foreach (string Transition in _Class.IncrementedAttributes)
                {
                    _Transitions[Transition] = 1 - _Transitions[Transition];
                }
            }
            else
            {
                foreach (string Transition in _Class.IncrementedAttributes)
                {
                    _Transitions[Transition] = 0;
                }
            }
        }

        public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
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
