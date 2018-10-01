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
            Function f = block.GetFunc(FunctionName);
            Operand[] a = new Operand[Args.Length];
            for(int i = 0; i < a.Length; i++) {
                a[i] = Args[i].Eval(block);
            }
            return f.Call(block, a);
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
