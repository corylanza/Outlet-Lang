using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Expressions {
    public class Binary : Expression {

        private Expression left, right;
        private Operator op;

        public Binary(Expression left, Operator op, Expression right) {
            this.left = left;
            this.right = right;
            this.op = op;
        }

        public override Operand Eval() => null;// op.PerformOp(left.Eval, right.Eval());

        public override string ToString() {
            throw new NotImplementedException();
        }
    }
}
