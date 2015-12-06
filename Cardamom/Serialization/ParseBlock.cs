using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cardamom.Serialization
{
    public class ParseBlock
    {
        private string _Name;
        private string _String;
        private int _End;

        public string Name { get { return _Name; } }
        public string String { get { return _String; } }

        static char[] Delimiter = { '{', '}' };

        public ParseBlock(string Source)
        {
            _End = Source.Length;
            _String = Source;
        }

        public static ParseBlock FromFile(string Path) { return new ParseBlock(File.ReadAllText(Path)); }

        private ParseBlock(string Source, int i, char[] Delimiters)
        {
            int Start = Source.IndexOf(Delimiters[0], i);
            _Name = Source.Substring(i, Start - i).Trim();
            if (Delimiters[0] == Delimiters[1])
            {
                _End = Source.IndexOf(Delimiters[1], Start + 1);
            }
            else
            {
                int depth = 1;
                for (_End = Start + 1; _End < Source.Length; _End++)
                {
                    if (Source[_End] == Delimiters[0]) depth++;
                    else if (Source[_End] == Delimiters[1]) depth--;
                    if (depth == 0) break;
                }
            }
            Start++;
            _String = Source.Substring(Start, _End - Start).Trim();
        }

        public ParseBlock(string Source, int Start) : this(Source, Start, Delimiter) { }


        public List<ParseBlock> Break()
        {
            List<ParseBlock> Blocks = new List<ParseBlock>();
            int i = 0;
            while (i < _String.Length && _String.IndexOf(Delimiter[0], i) > -1)
            {
                ParseBlock newBlock = new ParseBlock(_String, i);
                Blocks.Add(newBlock);
                i = newBlock._End + 1;
            }
            return Blocks;
        }

        public List<ParseBlock> Break(char[] Delimiters)
        {
            List<ParseBlock> Blocks = new List<ParseBlock>();
            int i = 0;
            while (i < _String.Length && _String.IndexOf(Delimiters[0], i) > -1)
            {
                ParseBlock newBlock = new ParseBlock(_String, i, Delimiters);
                Blocks.Add(newBlock);
                i = newBlock._End + 1;
            }
            return Blocks;
        }
    }
}
