using Outlet.Operators;

namespace Outlet.AST {
	public class Binary : BinaryExpression {

		public readonly string Op;
		public BinaryOperation? Oper;
		public readonly Overload<BinaryOperation> Overloads;

        public Binary(string op, Expression left, Expression right, Overload<BinaryOperation> overloads) : base(left, right)
		{
			Overloads = overloads;
			Op = op;
        }

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"({Left} {Op} {Right})";
    }
}
