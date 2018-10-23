using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Deref : Expression {

		private readonly Expression Left;
		private readonly Identifier Right;

		public Deref(Expression left, Expression right) {
			Left = left;
			if (right is Identifier id) Right = id;
			else throw new OutletException("expected identifier following dereferencing " + left.ToString());
		}

		public override Operand Eval(Scope scope) {
			Operand temp = Left.Eval(scope);
			if (temp is IDereferenceable derefed) return derefed.Dereference(Right);
			else throw new OutletException("cannot dereference variable of type " + temp.Type);
		}

		public override void Resolve(Scope scope) {
			Left.Resolve(scope);
			Right.Resolve(scope);
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
