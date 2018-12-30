using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Is : Expression {

		public readonly Expression Left, Right;
		public readonly bool NotIsnt;

		public Is(Expression left, Expression right, bool yes) {
			(Left, Right, NotIsnt) = (left, right, yes);
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Left + " is " + Right; 
	}
}
