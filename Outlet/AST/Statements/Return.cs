using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ReturnStatement : Statement {

		public readonly Expression Expr;

		public ReturnStatement(Expression e) => Expr = e;

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => "return " + Expr.ToString();
	}
}
