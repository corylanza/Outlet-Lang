using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {
	public abstract class TokenLiteral : Token {

		public int LineNumber, CharPos;

		public TokenLiteral(int linenumber, int posinline) {
			LineNumber = linenumber;
			CharPos = posinline;
		}

		public dynamic Value;
		public override string ToString() => Value.ToString();
	}

	public class IntLiteral : TokenLiteral {
		public IntLiteral(string value, int line, int pos) : base(line, pos) {
			Value = int.Parse(value);
		}
	}
	public class FloatLiteral : TokenLiteral {
		public FloatLiteral(string value, int line, int pos) : base(line, pos) {
			Value = float.Parse(value);
		}
	}

	public class BoolLiteral : TokenLiteral {
		public BoolLiteral(string value, int line, int pos) : base(line, pos) {
			Value = bool.Parse(value);
		}
	}

	public class StringLiteral : TokenLiteral {
		public StringLiteral(string value, int line, int pos) : base(line, pos) {
			Value = value;
		}
	}

	public class NullLiteral : TokenLiteral {
		public NullLiteral(int line, int pos) : base(line, pos) {
			Value = null;
		}
	}
}
