using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {
	public abstract class Literal : Token {

		public int LineNumber, CharPos;

		public Literal(int linenumber, int posinline) {
			LineNumber = linenumber;
			CharPos = posinline;
		}

		public dynamic Value;
		public override string ToString() => Value.ToString();
	}

	public class IntLiteral : Literal {
		public IntLiteral(string value, int line, int pos) : base(line, pos) {
			Value = int.Parse(value);
		}
	}
	public class FloatLiteral : Literal {
		public FloatLiteral(string value, int line, int pos) : base(line, pos) {
			Value = float.Parse(value);
		}
	}

	public class BoolLiteral : Literal {
		public BoolLiteral(string value, int line, int pos) : base(line, pos) {
			Value = bool.Parse(value);
		}
	}

	public class StringLiteral : Literal {
		public StringLiteral(string value, int line, int pos) : base(line, pos) {
			Value = value;
		}
	}

	public class NullLiteral : Literal {
		public NullLiteral(int line, int pos) : base(line, pos) {
			Value = null;
		}
	}
}
