using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public abstract class Expression : Statement {

		public override void Execute(Scope block) => Eval(block);
		
		public abstract Operand Eval(Scope block);
    }

}
