using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {

    public abstract class TokenLiteral : Token
    {
        public int LineNumber, CharPos;

        protected TokenLiteral(int linenumber, int posinline)
        {
            LineNumber = linenumber;
            CharPos = posinline;
        }

        public abstract override string ToString();
    }

    public class NullLiteral : TokenLiteral
    {
        public NullLiteral(int line, int pos) : base(line, pos)
        { }

        public override string ToString() => "null";
    }

    public abstract class TokenLiteral<E> : TokenLiteral {

        protected TokenLiteral(E val, int linenumber, int posinline) : base(linenumber, posinline)
        {
            Value = val;
        }

        [NotNull]
        public E Value;
        public override string ToString() => $"{Value}";
	}

	public class IntLiteral : TokenLiteral<int> {
		public IntLiteral(string value, int line, int pos) : base(int.Parse(value), line, pos) { }
	}
	public class FloatLiteral : TokenLiteral<float> {
        public FloatLiteral(string value, int line, int pos) : base(float.Parse(value), line, pos) { }
	}

	public class BoolLiteral : TokenLiteral<bool> {
		public BoolLiteral(string value, int line, int pos) : base(bool.Parse(value), line, pos) { }
	}

	public class StringLiteral : TokenLiteral<string> {
		public StringLiteral(string value, int line, int pos) : base(value, line, pos) {}
	}
}
