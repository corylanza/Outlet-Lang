using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Expressions {
	public class Unary : Expression {
		private Expression input;
		private Operator op;

		public Unary(Expression input, Operator op) {
			this.input = input;
			this.op = op;
		}

		public override Operand Eval() => op.PerformOp(input.Eval());

		public override string ToString() => "("+op.ToString() + " " + input.ToString() + ")";
	}
}
