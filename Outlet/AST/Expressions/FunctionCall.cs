using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class FunctionCall : Expression {

		private string FunctionName;
		private Expression[] Args;

		public FunctionCall(Identifier function, params Expression[] args) {
			FunctionName = function.Name;
			Args = args;
		}

		public override Operand Eval(Scope block) {
            Function f = block.GetFunc(FunctionName);
            Operand[] a = new Operand[Args.Length];
            for(int i = 0; i < a.Length; i++) {
                a[i] = Args[i].Eval(block);
            }
            return f.Call(a);
		}

		public override string ToString() => FunctionName + new OTuple(Args).ToString(); 
	}
}
