using Outlet.Operators;

namespace Outlet.AST {
	public class Unary : Expression {

		public string Op;
		public Expression Expr;
		public UnaryOperation? Oper;
		public Overload<UnaryOperation> Overloads;

		public Unary(string op, Expression input, Overload<UnaryOperation> overloads) {
			Expr = input;
			Overloads = overloads;
			Op = op;
		}

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"({Op} {Expr})";
	}
}
