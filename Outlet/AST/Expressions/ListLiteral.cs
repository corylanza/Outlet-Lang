using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ListLiteral : Expression {

		private readonly Expression[] Args;

		public ListLiteral(params Expression[] vals) {
			Args = vals;
		}

		public override Operand Eval(Scope scope) => new OList(Args.Select(arg => arg.Eval(scope)).ToArray());

		public override void Resolve(Scope scope) {
			foreach (Expression e in Args) e.Resolve(scope);
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
