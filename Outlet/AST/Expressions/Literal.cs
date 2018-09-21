using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Literal : Operand, IToken {
		public Literal(int value) {
			Value = value;
		}

		public Literal(string value) {
			Value = value;
		}

		public Literal(float value) {
			Value = value;
		}

		public Literal(bool value) {
			Value = value;
		}

		public override Operand Eval() => this;

		public override string ToString() => Value.ToString();
	}
}
