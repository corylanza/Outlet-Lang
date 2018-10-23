using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Assign : Expression {

		private readonly Expression Left;
		private readonly Expression Right;

		public Assign(Expression left, Expression right) {
			Left = left;
			Right = right;
		}

		public override Operand Eval(Scope scope) {
			Operand var = Left.Eval(scope);
			Operand val = Right.Eval(scope);
			var.Value = val.Value;
			return var;
		}

		public override void Resolve(Scope scope) {
			Left.Resolve(scope);
			Right.Resolve(scope);
		}

		public override string ToString() => Left.ToString() + " = " + Right.ToString();
	}
}
