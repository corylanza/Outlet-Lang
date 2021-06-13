using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Is : BinaryExpression {

		public readonly bool NotIsnt;

		public Is(Expression left, Expression right, bool yes) : base(left, right) {
			NotIsnt = yes;
		}

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => Left + (NotIsnt ? " is " : " isnt ") + Right; 
	}
}
