using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Assign : Expression {

		public readonly Expression Left, Right;

		public Assign(Expression left, Expression right) {
			Left = left;
			Right = right;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Left.ToString() + " = " + Right.ToString();
	}
}
