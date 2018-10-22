using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class FunctionCall : Expression {

		private Identifier FunctionName;
		private Expression[] Args;

		public FunctionCall(Identifier function, params Expression[] args) {
			FunctionName = function;
			Args = args;
		}

		public override Operand Eval(Scope block) {
            ICallable f = FunctionName.Eval(block) as ICallable;
            Operand[] a = new Operand[Args.Length];
            for(int i = 0; i < a.Length; i++) {
                a[i] = Args[i].Eval(block);
            }
            return f.Call(block, a);
		}

        public override void Resolve(Scope block) {
			FunctionName.Resolve(block);
            foreach(Expression e in Args) e.Resolve(block);
        }

        public override string ToString() => FunctionName.Name + new OTuple(Args).ToString(); 
	}
}
