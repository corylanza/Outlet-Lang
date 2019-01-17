using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Ternary : Expression {

		public readonly Expression Condition, IfTrue, IfFalse;

		public Ternary(Expression iffalse, Expression iftrue, Expression condition) {
			Condition = condition;
			IfTrue = iftrue;
			IfFalse = iffalse;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Condition.ToString() + " ? " + IfTrue.ToString() + " : " + IfFalse.ToString();
	}
}
