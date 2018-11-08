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

		public override Operand Eval(Scope scope) {
			if(Condition.Eval(scope).Value is bool b) {
				return b ? IfTrue.Eval(scope) : IfFalse.Eval(scope);
			}
			throw new OutletException("expected boolean in ternary condition");
		}

		public override void Resolve(Scope scope) {
			Condition.Resolve(scope);
			IfTrue.Resolve(scope);
			IfFalse.Resolve(scope);
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Condition.ToString() + " ? " + IfTrue.ToString() + " : " + IfFalse.ToString();
	}
}
