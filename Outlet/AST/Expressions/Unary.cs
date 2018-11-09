using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;

namespace Outlet.AST {
	public class Unary : Expression {
		public Expression Expr;
		public UnaryOperation Oper;
		public Overload<UnaryOperation> Overloads;
		public UnaryOperator Op;

		public Unary(Expression input, UnaryOperator op, Overload<UnaryOperation> overloads) {
			Expr = input;
			Overloads = overloads;
			Op = op;
		}

		public override Operand Eval(Scope scope) => Oper.Perform(Expr.Eval(scope));//Op.PerformOp(Expr.Eval(scope));

        public override void Resolve(Scope scope) {
            Expr.Resolve(scope);
        }

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "("+Op.ToString() + " " + Expr.ToString() + ")";
	}
}
