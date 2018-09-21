using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class FunctionCall : Expression {

		Identifier FunctionName;
		Expression[] Args;

		public FunctionCall(Identifier function, params Expression[] args) {
			FunctionName = function;
			Args = args;
		}

		public override Operand Eval() {
			Function f = new Function();
			Operand[] a = new Operand[Args.Length];
			for(int i = 0; i < a.Length; i++) {
				a[i] = Args[i].Eval();
			}
			f.Eval(a);
			return null;
		}

		public override string ToString() => FunctionName.Name + new OTuple(Args).ToString(); 
	}
}
