using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class FunctionCall : Expression {

		private Expression Caller;
		private Expression[] Args;

		public FunctionCall(Expression caller, params Expression[] args) {
			Caller = caller;
			Args = args;
		}

		public override Operand Eval(Scope scope) {
			Operand Left = Caller.Eval(scope);
			if (Left is ICallable c) {
				return c.Call(Args.Select(arg => arg.Eval(scope)).ToArray());
			} else throw new OutletException(Left.Type.ToString() + " is not callable");
		}

        public override void Resolve(Scope scope) {
			Caller.Resolve(scope);
            foreach(Expression e in Args) e.Resolve(scope);
        }

        public override string ToString() => Caller.ToString(); 
	}
}
