using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Lexer
{
    public partial class Lexer
    {
        protected static CharGroup Or(params CharGroup[] groups) => (char c) => groups.Any(group => group(c));

        protected static CharGroup Not(CharGroup group) => NotAnyOf(group);

        protected static CharGroup NotAnyOf(params CharGroup[] groups) => (char c) => !groups.Any(group => group(c));

        protected static readonly CharGroup Letter = (char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');

        protected static readonly CharGroup Number = (char c) => c is >= '0' and <= '9';

        protected static readonly CharGroup WhiteSpace = (char c) => c is ' ' or '\t';

        protected static readonly CharGroup NewLine = (char c) => c is '\n' or '\r';

        protected static readonly CharGroup Dot = (char c) => c is '.';

        protected static readonly CharGroup DoubleQuote = (char c) => c is '"';

        protected static readonly CharGroup Delimeter = (char c) => c is '(' or ')' or '[' or ']' or '{' or '}' or ':' or ';' or ',' or '?';

        protected static readonly CharGroup PreEquals = (char c) => c is '+' or '-' or '*' or '/' or '%' or '!' or '<' or '>' or '^' or '&' or '|' or '=';

        protected static readonly CharGroup EqualsSign = (char c) => c is '=';
    }
}
