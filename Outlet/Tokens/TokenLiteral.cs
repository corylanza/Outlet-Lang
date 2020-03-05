using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {

    public abstract class TokenLiteral : Token
    {
        public int LineNumber, CharPos;

        public TokenLiteral(int linenumber, int posinline)
        {
            LineNumber = linenumber;
            CharPos = posinline;
        }

        public abstract override string ToString();
    }

	public abstract class TokenLiteral<E> : TokenLiteral {

        public TokenLiteral(int linenumber, int posinline) : base(linenumber, posinline)
        {
        }

        public E Value;
		public override string ToString() => Value is null ? "null" : Value.ToString();
	}

	public class IntLiteral : TokenLiteral<int> {
		public IntLiteral(string value, int line, int pos) : base(line, pos) {
			Value = int.Parse(value);
		}
	}
	public class FloatLiteral : TokenLiteral<float> {
		public FloatLiteral(string value, int line, int pos) : base(line, pos) {
			Value = float.Parse(value);
		}
	}

	public class BoolLiteral : TokenLiteral<bool> {
		public BoolLiteral(string value, int line, int pos) : base(line, pos) {
			Value = bool.Parse(value);
		}
	}

	public class StringLiteral : TokenLiteral<string> {
		public StringLiteral(string value, int line, int pos) : base(line, pos) {
			Value = value;
		}
	}

	public class NullLiteral : TokenLiteral<object> {
		public NullLiteral(int line, int pos) : base(line, pos) {
			//Value = null;
		}
	}
}
