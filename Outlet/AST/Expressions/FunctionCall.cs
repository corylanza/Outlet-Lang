using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class FunctionCall : Expression {

		private Identifier FunctionName;
		private Expression[] Args;

		public FunctionCall(Scope s, Identifier function, params Expression[] args) {
			FunctionName = function;
			Args = args;
		}

		public override Operand Eval(Scope block) {
            Function f = block.Get(FunctionName) as Function;
            Operand[] a = new Operand[Args.Length];
            for(int i = 0; i < a.Length; i++) {
                a[i] = Args[i].Eval(block);
            }
            return f.Call(block, a);
		}

		public override string ToString() => FunctionName.Name + new OTuple(Args).ToString(); 
	}
}
