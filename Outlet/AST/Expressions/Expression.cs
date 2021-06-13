using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public abstract class Expression : Statement {

		public abstract override string ToString();
	}

	public abstract class BinaryExpression : Expression
    {
		public Expression Left { get; private init; }
		public Expression Right { get; private init; }

		protected BinaryExpression(Expression left, Expression right) => (Left, Right) = (left, right);
    }

}