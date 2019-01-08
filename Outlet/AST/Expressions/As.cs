using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class As : Expression {
		public readonly Expression Left, Right;

		public As(Expression left, Expression right) {
			(Left, Right) = (left, right);
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Left + " as " + Right;
	}
}
