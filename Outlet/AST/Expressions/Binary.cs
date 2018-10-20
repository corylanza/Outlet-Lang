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

		public override Operand Eval(Scope block) {
			if (op == Operator.Dot) {
				if (left.Eval(block) is IDereferenceable l && right is Identifier r) {
					return l.Dereference(r);
				} else throw new OutletException("invalid dereference: ");
			}
			
			return op.PerformOp(left.Eval(block), right.Eval(block));
		}

        public override void Resolve(Scope block) {
            left.Resolve(block);
            right.Resolve(block);
        }

        public override string ToString() => "("+left.ToString() +" "+ op.ToString() + " "+right.ToString()+")";
    }
}
