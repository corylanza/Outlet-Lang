using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Assign : Expression {

		private readonly Expression Left, Right;

		public Assign(Expression left, Expression right) {
			Left = left;
			Right = right;
		}

		public override Operand Eval(Scope scope) {
			if(Left is Identifier id) {
				Operand val = Right.Eval(scope);
				scope.Assign(id.resolveLevel, id.Name, val);
				return val;
			}
			throw new OutletException("invalid left side of assignment expression");
		}

		public override void Resolve(Scope scope) {
			Left.Resolve(scope);
			Right.Resolve(scope);
		}

		public override string ToString() => Left.ToString() + " = " + Right.ToString();
	}
}
