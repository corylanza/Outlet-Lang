using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Unary : Expression {

		public string Op;
		public Expression Expr;
		public UnOp? Oper;
		public Overload<UnOp> Overloads;

		public Unary(string op, Expression input, Overload<UnOp> overloads) {
			Expr = input;
			Overloads = overloads;
			Op = op;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "("+Op.ToString() + " " + Expr.ToString() + ")";
	}
}
