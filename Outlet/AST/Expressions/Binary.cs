using Outlet.Operators;

namespace Outlet.AST {
	public class Binary : Expression {

		public readonly string Op;
        public readonly Expression Left, Right;
		public BinaryOperation? Oper;
		public Overload<BinaryOperation> Overloads;

        public Binary(string op, Expression left, Expression right, Overload<BinaryOperation> overloads) {
            Left = left;
            Right = right;
			Overloads = overloads;
			Op = op;
        }

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "("+Left.ToString() +" "+ Op + " "+Right.ToString()+")";
    }
}
