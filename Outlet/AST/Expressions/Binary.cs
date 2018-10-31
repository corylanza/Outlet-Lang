using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Binary : Expression {

        private readonly Expression left, right;
        private readonly BinaryOperator op;

        public Binary(Expression left, BinaryOperator op, Expression right) {
            this.left = left;
            this.right = right;
            this.op = op;
        }

		public override Operand Eval(Scope scope) {
			return op.PerformOp(left.Eval(scope), right.Eval(scope));
		}

        public override void Resolve(Scope scope) {
            left.Resolve(scope);
            right.Resolve(scope);
        }

        public override string ToString() => "("+left.ToString() +" "+ op.ToString() + " "+right.ToString()+")";
    }
}
