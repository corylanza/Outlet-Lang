using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class FunctionCall : Expression {

		private Identifier FunctionName;
		private Expression[] Args;
        private Scope Scope;

		public FunctionCall(Scope s, Identifier function, params Expression[] args) {
			FunctionName = function;
			Args = args;
            Scope = s;
		}

		public override Operand Eval() {
            Function f = Scope.GetFunc(FunctionName);
            Operand[] a = new Operand[Args.Length];
            for(int i = 0; i < a.Length; i++) {
                a[i] = Args[i].Eval();
            }
            return f.Call(a);
            /*
			Function f = new Function();
			Operand[] a = new Operand[Args.Length];
			for(int i = 0; i < a.Length; i++) {
				a[i] = Args[i].Eval();
			}
			try {
				f.Call(a);
			} catch (Return r) {
				return r.Value;
			}
			return null;*/
		}

		public override string ToString() => FunctionName.Name + new OTuple(Args).ToString(); 
	}
}
