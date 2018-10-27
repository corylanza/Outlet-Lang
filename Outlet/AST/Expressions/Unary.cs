using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Unary : Expression {
		private Expression input;
		private UnaryOperator op;

		public Unary(Expression input, UnaryOperator op) {
			this.input = input;
			this.op = op;
		}

		public override Operand Eval(Scope scope) => op.PerformOp(input.Eval(scope));

        public override void Resolve(Scope scope) {
            input.Resolve(scope);
        }

        public override string ToString() => "("+op.ToString() + " " + input.ToString() + ")";
	}
}
