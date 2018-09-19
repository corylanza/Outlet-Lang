using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Expressions {
	public class FunctionCall : Expression {

		Identifier FunctionName;
		Expression[] Args;

		public FunctionCall(Identifier function, params Expression[] args) {
			FunctionName = function;
			Args = args;
		}

		public override Operand Eval() {
			throw new NotImplementedException();
		}

		public override string ToString() => FunctionName.Name + new OTuple(Args).ToString(); 
	}
}
