using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using SFML.Graphics;

namespace Cardamom.Interface
{
    public class LoadingScreen : Screen
    {
        private volatile bool _Done;
        private float _Progress;
        private Action _Load;
        private bool _Started;

        public EventHandler Finished;

        public float Progress { get { return _Progress; } }
        public bool Done { get { return _Done; } }
        public bool Started { get { return _Started; } }

        public LoadingScreen() : base() { }

        public void AddProgress(double Add) { _Progress += (float)Add; }
        public void SetAction(Action Action) { _Load = Action; }
        public void SetAction(NLua.LuaFunction Action, object args) { _Load = delegate() { Action.Call(args); Finish(); }; }
        private void Finish() { _Done = true; if (_Done != false) Finished(this, EventArgs.Empty); }

        public LoadingScreen(string LoaderScript, NLua.Lua LuaInstance)
            : base()
        {
            LuaInstance.GetFunction(LoaderScript).Call(this);
        }

        public void Start()
        {
            Thread t = new Thread(new ThreadStart(_Load));
            t.Start();
            _Started = true;
        }
    }
}
