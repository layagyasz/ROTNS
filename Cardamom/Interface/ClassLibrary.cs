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
		public static readonly int[] NullArray = new int[] { 0, 0, 0, 0 };
		public static readonly Vector2f NullVector = new Vector2f(0, 0);
		public static readonly byte[] NullBytes = new byte[] { 0, 0, 0, 0 };

		private static readonly ClassLibrary _ClassLibrary = new ClassLibrary();
		private ClassLibrary() { }
		public static ClassLibrary Instance { get { return _ClassLibrary; } }

		Dictionary<string, Class> _Classes;

		Dictionary<string, Func<XmlReader, XmlReadable>> _ItemGenerators = new Dictionary<string, Func<XmlReader, XmlReadable>>();

		public void ReadFile(string Path)
		{
			ReadBlock(new ParseBlock(File.ReadAllText(Path)));
		}

		public void ReadBlock(ParseBlock Block)
		{
			Block.AddParser<Color>("color", i => ClassLibrary.Instance.ParseColor(i.String), false);
			Block.AddParser<List<Color>>("color[]", i => ClassLibrary.Instance.ParseColors(i.String), false);
			Block.AddParser<Dictionary<string, Color>>("color<>", i => i.BreakToDictionary<Color>(), false);
			Block.AddParser<Vector2f>("vector2f", i => ClassLibrary.Instance.ParseVector2f(i.String), false);
			Block.AddParser<Vector2f[]>("vector2f[]", i => ClassLibrary.Instance.ParseVector2fs(i.String), false);
			Block.AddParser<Font>("font", i => ClassLibrary.Instance.ParseFont(i.String));
			Block.AddParser<Sound>("sound", i => ClassLibrary.Instance.ParseSound(i.String));
			Block.AddParser<Texture>("texture", i => ClassLibrary.Instance.ParseTexture(i.String));
			Block.AddParser<Class>("class", i => new Class(i));
			Block.AddParser<SubClass>("mode", i => new SubClass(i));

			_Classes = (Dictionary<string, Class>)Block.BreakToDictionary<object>(true)["classes"];
		}

		public void AddItemGenerator(string Type, Func<XmlReader, XmlReadable> Constructor) { _ItemGenerators.Add(Type, Constructor); }
		public XmlReadable GenerateItem(string Type, XmlReader Reader) { return _ItemGenerators[Type].Invoke(Reader); }

		public Class this[string Name]
		{
			get { return _Classes[Name]; }
		}

		public Texture ParseTexture(string Code)
		{
			return new Texture(Code);
		}

		public Sound ParseSound(string Code)
		{
			return new Sound(new SoundBuffer(Code));
		}

		public Font ParseFont(string Code)
		{
			return new Font(Code);
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
					if (Code.Length == 7)
					{
						return new Color(
							Convert.ToByte(Code.Substring(1, 2), 16),
							Convert.ToByte(Code.Substring(3, 2), 16),
							Convert.ToByte(Code.Substring(5, 2), 16)
						);
					}
					else
					{
						return new Color(
							Convert.ToByte(Code.Substring(1, 2), 16),
							Convert.ToByte(Code.Substring(3, 2), 16),
							Convert.ToByte(Code.Substring(5, 2), 16),
							Convert.ToByte(Code.Substring(7, 2), 16)
						);
					}
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
				Console.WriteLine("ERROR READING COLOR: {0}", Code);
				return NullColor;
			}
		}
	}
}
