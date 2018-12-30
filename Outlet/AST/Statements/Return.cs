using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ReturnStatement : Statement {

		public readonly Expression Expr;

		public ReturnStatement(Expression e) {
			Expr = e;
		}
		/*
		public override void Resolve(Scope scope) => Expr.Resolve(scope);

		public override void Execute(Scope scope) => throw new Return(Expr.Eval(scope));
		*/
		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "return " + Expr.ToString();
	}

	public class Return : OutletException {

		public Operand Value;

		public Return(Operand o) {
			Value = o;
		}
	}
}
