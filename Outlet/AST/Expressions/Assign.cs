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

		public override Operand Eval(Scope scope) {
			if(Left is IAssignable id) {
				Operand val = Right.Eval(scope);
				id.Assign(scope, val);
				return val;
			} throw new OutletException("cannot assign to the left side of this expression");
		}

		public override void Resolve(Scope scope) {
			Left.Resolve(scope);
			Right.Resolve(scope);
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Left.ToString() + " = " + Right.ToString();
	}
}
