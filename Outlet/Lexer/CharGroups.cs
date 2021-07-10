using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Lexer
{
    public partial class Lexer
    {
        public record CharGroup(HashSet<char> Chars)
        {
            public CharGroup(params char[] chars) : this(new CharGroup(chars.ToHashSet())) { }

            public bool Contains(char c) => Chars.Contains(c);
        };

        static CharGroup Chars(params char[] chars) => new CharGroup(chars);
        static CharGroup Chars(IEnumerable<char> chars) => new CharGroup(chars.ToArray());
        static CharGroup Range(char start, char stop) => Chars(Enumerable.Range(start, stop - start + 1).Select(x => (char) x));

        protected static readonly CharGroup AllChars = Range(char.MinValue, char.MaxValue);

        protected static CharGroup Or(params CharGroup[] groups) => Chars(groups.SelectMany(c => c.Chars));

        protected static CharGroup Not(CharGroup group) => Chars(AllChars.Chars.Except(group.Chars));

        protected static CharGroup NotAnyOf(params CharGroup[] groups) => Chars(AllChars.Chars.Except(Or(groups).Chars));

        protected static readonly CharGroup Letter = Or(Range('a', 'z'), Range('A', 'Z'));

        protected static readonly CharGroup Number = Range('0', '9');//(char c) => c is >= '0' and <= '9';


        protected static readonly CharGroup Zero = Chars('0');
        protected static readonly CharGroup X = Chars('x', 'X');
        protected static readonly CharGroup Hex = Or(Range('0', '9'), Range('a', 'f'), Range('A', 'F'));

        protected static readonly CharGroup WhiteSpace = Chars(' ', '\t');

        protected static readonly CharGroup NewLine = Chars('\n', '\r');

        protected static readonly CharGroup Dot = Chars('.');

        protected static readonly CharGroup Underscore = Chars('_');

        protected static readonly CharGroup DoubleQuote = Chars('"');

        protected static readonly CharGroup Delimeter = Chars('(', ')', '[', ']', '{', '}', ':', ';', ',', '?');

        protected static readonly CharGroup Plus = Chars('+');

        protected static readonly CharGroup Minus = Chars('-');

        protected static readonly CharGroup LAngleBracket = Chars('<');

        protected static readonly CharGroup RAngleBracket = Chars('>');

        protected static readonly CharGroup Ampsersand = Chars('&');

        protected static readonly CharGroup Pipe = Chars('|');

        protected static readonly CharGroup OneCharOp = Chars('~', '#', '.');

        protected static readonly CharGroup PreEquals = Chars('+', '-', '*', '/', '%', '!', '<', '>', '^', '&', '|', '=');

        protected static readonly CharGroup EqualsSign = Chars('=');
    }
}
