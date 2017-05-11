using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

using Cardamom.Serialization;

namespace Cardamom.Interface
{
    public class Screen : Container<Pod>
    {
        protected LoadingScreen _Loader;

        public Screen() { }

        public Screen(string ScriptName, NLua.Lua LuaInstance)
            : this()
        {
            LuaInstance.GetFunction(ScriptName).Call(this);
        }

        public Screen(string ScriptName, string LoaderScript, NLua.Lua LuaInstance)
            : this()
        {
            LuaInstance.GetFunction(ScriptName).Call(this);
            _Loader = new LoadingScreen(LoaderScript, LuaInstance);
        }

        public Screen(ParseBlock Block)
            : this()
        {
            using (XmlTextReader Reader = new XmlTextReader(Block.String))
            {
                while (Reader.Read())
                {
                    Add((Pod)ClassLibrary.Instance.GenerateItem(Reader.Name, Reader));
                }
            }
        }


        public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
        {
            if (_Loader != null && !_Loader.Started) _Loader.Start();

            if (_Loader == null || _Loader.Done)
            {
                base.Update(MouseController, KeyController, DeltaT, Transform);
            }
            else _Loader.Update(MouseController, KeyController, DeltaT, Transform);
        }

        public override void Draw(RenderTarget Target, Transform Transform)
        {
            if (_Loader != null && !_Loader.Started) _Loader.Start();

            if (_Loader == null || _Loader.Done)
            {
                base.Draw(Target, Transform);
            }
            else _Loader.Draw(Target, Transform);
        }
    }
}
