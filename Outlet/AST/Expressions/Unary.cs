using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;

namespace Outlet.AST {
	public class Unary : Expression {
		public Expression Expr;
		public UnaryOperator Op;

		public Unary(Expression input, UnaryOperator op) {
			this.Expr = input;
			this.Op = op;
		}

		public override Operand Eval(Scope scope) => Op.PerformOp(Expr.Eval(scope));

        public override void Resolve(Scope scope) {
            Expr.Resolve(scope);
        }

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "("+Op.ToString() + " " + Expr.ToString() + ")";
	}
}
