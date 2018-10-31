using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Lambda : Expression {

		public Lambda(Expression l, Expression r) {

		}

		public override Operand Eval(Scope scope) {
			//Function f = new Function(scope, "", )
			throw new NotImplementedException();
		}

		public override void Resolve(Scope scope) {
			Scope exec = new Scope(scope);
			throw new NotImplementedException();
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
