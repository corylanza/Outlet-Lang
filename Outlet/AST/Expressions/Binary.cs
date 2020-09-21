using Outlet.Operators;

namespace Outlet.AST {
	public class Binary : Expression {

		public readonly string Op;
        public readonly Expression Left, Right;
		public BinOp? Oper;
		public Overload<BinOp> Overloads;

        public Binary(string op, Expression left, Expression right, Overload<BinOp> overloads) {
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
