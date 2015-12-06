using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using SFML.Graphics;
using SFML.Audio;
using SFML.Window;

using Cardamom.Serialization;
using Cardamom.Planar;

namespace Cardamom.Interface
{
    public class ClassLibrary
    {
        public static readonly Color NullColor = new Color(0, 0, 0, 0);
        public static readonly Color[] NullColors = new Color[] { NullColor, NullColor, NullColor, NullColor };
        public static readonly Vector2f NullVector = new Vector2f(0, 0);
        public static readonly byte[] NullBytes = new byte[] { 0, 0, 0, 0 };

        private static readonly ClassLibrary _ClassLibrary = new ClassLibrary();
        private ClassLibrary() { }
        public static ClassLibrary Instance { get { return _ClassLibrary; } }

        Dictionary<string, Class> _Classes = new Dictionary<string, Class>();
        Dictionary<string, Sound> _Sounds = new Dictionary<string, Sound>();
        Dictionary<string, Color> _Colors = new Dictionary<string, Color>();
        Dictionary<string, Font> _Fonts = new Dictionary<string, Font>();

        Dictionary<string, Func<XmlReader, XmlReadable>> _ItemGenerators = new Dictionary<string, Func<XmlReader, XmlReadable>>();

        public void ReadFile(string Path)
        {
            ReadBlock(new ParseBlock(File.ReadAllText(Path)));
        }

        public void ReadBlock(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                switch (B.Name.ToLower())
                {
                    case "classes": ReadClasses(B); break;
                    case "fonts": ReadFonts(B); break;
                    case "colors": ReadColors(B); break;
                    case "sounds": ReadSounds(B); break;
                }
            }
        }

        private void ReadClasses(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                string[] def = B.Name.Split(':');
                string Name = "";
                if (def.Length > 1) Name = def[1].Trim();
                else Name = def[0].Trim();

                Class newClass = new Class(B);
                _Classes.Add(Name, newClass);
            }
        }

        private void ReadSounds(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                _Sounds.Add(B.Name, new Sound(new SoundBuffer(Block.String)));
            }
        }

        private void ReadFonts(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                _Fonts.Add(B.Name, new Font(B.String));
            }
        }

        private void ReadColors(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                _Colors.Add(B.Name, ParseColor(B.String));
            }
        }

        public void AddItemGenerator(string Type, Func<XmlReader, XmlReadable> Constructor) { _ItemGenerators.Add(Type, Constructor); }
        public XmlReadable GenerateItem(string Type, XmlReader Reader) { return _ItemGenerators[Type].Invoke(Reader); }

        public Class this[string Name]
        {
            get { return _Classes[Name]; }
        }

        public Sound ParseSound(string Code)
        {
            if (Code[0] == '&') return new Sound(new SoundBuffer(Code.Substring(1)));
            else return _Sounds[Code];
        }

        public Font ParseFont(string Code)
        {
            if (Code[0] == '&') return new Font(Code);
            else return _Fonts[Code];
        }

        public Vector2f[] ParseVector2fs(string Code)
        {
            return Serialization.Parse.Array(Code, ParseVector2f);
        }

        public Vector2f ParseVector2f(string Code)
        {
            string[] def = Code.Split(',');
            return new Vector2f(Convert.ToSingle(def[0], System.Globalization.CultureInfo.InvariantCulture), Convert.ToSingle(def[1], System.Globalization.CultureInfo.InvariantCulture));
        }

        public Collision ParseCollision(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                switch (B.Name.ToLower())
                {
                    case "circle": return new CollisionRadius(Block);
                }
            }
            return new CollisionPolygon(Block);
        }

        public Color[] ParseColors(string Code)
        {
            return Serialization.Parse.Array(Code, ParseColor);
        }

        public Color ParseColor(string Code)
        {
            Code = Code.Trim();
            try
            {
                if (Code[0] == '#')
                {
                    return new Color(
                        Convert.ToByte(Code.Substring(1, 2), 16),
                        Convert.ToByte(Code.Substring(3, 2), 16),
                        Convert.ToByte(Code.Substring(5, 2), 16)
                    );
                }
                else
                {
                    string[] bytes = Code.Split(',');
                    return new Color(
                        Convert.ToByte(bytes[0]),
                        Convert.ToByte(bytes[1]),
                        Convert.ToByte(bytes[2]),
                        (byte)((bytes.Length > 3) ? Convert.ToByte(bytes[3]) : 255)
                    );
                }
            }
            catch
            {
                try
                {
                    return _Colors[Code];
                }
                catch
                {
                    return NullColor;
                }
            }
        }
    }
}
