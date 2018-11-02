using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {
	public abstract class Literal : Token {
		public dynamic Value;
	}

	public class IntLiteral : Literal {
		public IntLiteral(string value) {
			Value = int.Parse(value);
		}
	}
	public class FloatLiteral : Literal {
		public FloatLiteral(string value) {
			Value = float.Parse(value);
		}
	}
	public class StringLiteral : Literal {
		public StringLiteral(string value) {
			Value = value;
		}
	}
}
