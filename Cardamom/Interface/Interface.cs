using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using System.Diagnostics;
using System.Threading;
using SFML.Window;

using Cardamom.Planar;

namespace Cardamom.Interface
{
    public class Interface
    {
        MouseController _MouseController;
        KeyController _KeyController;
        RenderWindow _Window;
        Screen _Screen;
        bool Running = true;
        bool Paused = false;

        public Window Window { get { return _Window; } }
        public Screen Screen { get { return _Screen; } set { _Screen = value; } }
        public MouseController MouseController { get { return _MouseController; } }
        public KeyController KeyController { get { return _KeyController; } }

        double _UPS;
        double _FPS;
        public double UPS { get { return _UPS; } }
        public double FPS { get { return _FPS; } }

        Vector2f[] _WindowBounds;
        public Vector2f[] WindowBounds { get { return _WindowBounds; } }

        public Interface() { }

        public Interface(VideoMode VideoMode, string Title, Styles Style)
        {
            _Window = new RenderWindow(VideoMode, Title, Style);
            _WindowBounds = new Vector2f[] { new Vector2f(0, 0), new Vector2f(_Window.Size.X, 0), new Vector2f(_Window.Size.X, _Window.Size.Y), new Vector2f(0, _Window.Size.Y) };
            _MouseController = new MouseController(_Window);
            _KeyController = new KeyController();
        }

        public void Close(object sender, EventArgs e)
        {
            Running = false;
            ((RenderWindow)sender).Close();
        }
        private void Pause(object sender, EventArgs e)
        {
            Paused = true;
        }
        private void Halt(object sender, EventArgs e)
        {
            Running = false;
        }
        private void Resume(object sender, EventArgs e)
        {
            Paused = false;
        }
        private void Resized(object sender, SizeEventArgs e)
        {
            _MouseController = new MouseController((RenderWindow)sender);
        }

        private void UpdateThread()
        {
            int DeltaT = 0;
            long time = 0;
            Stopwatch w = new Stopwatch();
            w.Start();
            while (Running)
            {
                while (w.ElapsedMilliseconds - time < 10) ;
                long t = w.ElapsedMilliseconds;
                if (t <= time) t = time + 1;
                DeltaT = (int)(t - time);
                time = t;

                UpdateCall(DeltaT);
            }
        }

        private void UpdateCall(int DeltaT)
        {
            _UPS = 1000f / (float)DeltaT;

            _KeyController.Update(DeltaT);
            _MouseController.Update(_Window);

            _Screen.Update(_MouseController, _KeyController, DeltaT, Transform.Identity);
        }

        public void Start(bool MultiThread = true, bool ClearScreen = true)
        {
            if (MultiThread)
            {
                Thread Update = new Thread(new ThreadStart(UpdateThread));
                Update.Start();
            }

            _Window.GainedFocus += new EventHandler(Resume);
            _Window.LostFocus += new EventHandler(Pause);
            _Window.Closed += new EventHandler(Close);
            _Window.Resized += new EventHandler<SizeEventArgs>(Resized);

            int DeltaT = 0;
            long time = 0;
            Stopwatch w = new Stopwatch();
            w.Start();
            while (Running)
            {
				while (w.ElapsedMilliseconds - time < 10) { }
                long t = w.ElapsedMilliseconds;
                if (t <= time) t = time + 1;
                DeltaT = (int)(t - time);
                time = t;
                _FPS = 1000f / (float)DeltaT;

                _Window.DispatchEvents();
                if(ClearScreen) _Window.Clear();
                if (!MultiThread) UpdateCall(DeltaT);
                _Screen.Draw(_Window, Transform.Identity);
                _Window.Display();
            }
        }
    }
}
