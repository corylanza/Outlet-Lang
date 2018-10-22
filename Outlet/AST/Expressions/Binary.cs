using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public class Binary : Expression {

        private Expression left, right;
        private Operator op;

        public Binary(Expression left, Operator op, Expression right) {
            this.left = left;
            this.right = right;
            this.op = op;
        }

		public override Operand Eval(Scope scope) {
			if (op == Operator.Dot) {
				if (left.Eval(scope) is IDereferenceable l && right is Identifier r) {
					return l.Dereference(r);
				} else throw new OutletException("invalid dereference: ");
			}
			
			return op.PerformOp(left.Eval(scope), right.Eval(scope));
		}

        public override void Resolve(Scope scope) {
            left.Resolve(scope);
            right.Resolve(scope);
        }

        public override string ToString() => "("+left.ToString() +" "+ op.ToString() + " "+right.ToString()+")";
    }
}
