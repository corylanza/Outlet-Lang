using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public abstract class Expression : Statement {

		public override void Execute(Scope scope) => Eval(scope);
		
		public abstract Operand Eval(Scope scope);

		public abstract override string ToString();
	}

}
