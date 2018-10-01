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

        public override Operand Eval(Scope block) => op.PerformOp(left.Eval(block), right.Eval(block));

		public override string ToString() => "("+left.ToString() +" "+ op.ToString() + " "+right.ToString()+")";
    }
}
