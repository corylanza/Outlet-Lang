using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Unary : Expression {
		private Expression input;
		private Operator op;

		public Unary(Expression input, Operator op) {
			this.input = input;
			this.op = op;
		}

		public override Operand Eval(Scope block) => op.PerformOp(input.Eval(block));

		public override string ToString() => "("+op.ToString() + " " + input.ToString() + ")";
	}
}
