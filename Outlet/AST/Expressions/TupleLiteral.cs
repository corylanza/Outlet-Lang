using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class TupleLiteral : Expression {

		private readonly Expression[] Args;

		public TupleLiteral(params Expression[] vals) {
			Args = vals;
		}

		public override Operand Eval(Scope scope) {
			if (Args.Length == 1) return Args[0].Eval(scope);
			else return new OTuple(Args.Select(arg => arg.Eval(scope)).ToArray());
		}

		public override void Resolve(Scope scope) {
			foreach (Expression e in Args) e.Resolve(scope);
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
