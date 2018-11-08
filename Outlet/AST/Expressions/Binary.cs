using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;

namespace Outlet.AST {
	public class Binary : Expression {

        public readonly Expression Left, Right;
        private readonly BinaryOperator op;

        public Binary(Expression left, BinaryOperator op, Expression right) {
            Left = left;
            Right = right;
            this.op = op;
        }

		public override Operand Eval(Scope scope) {
			return op.PerformOp(Left.Eval(scope), Right.Eval(scope));
		}

        public override void Resolve(Scope scope) {
            Left.Resolve(scope);
            Right.Resolve(scope);
        }

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "("+Left.ToString() +" "+ op.ToString() + " "+Right.ToString()+")";
    }
}
