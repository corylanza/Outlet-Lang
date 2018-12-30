using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public abstract class Expression : Statement {

		public int Line, Pos;

		public Expression(int line=0, int pos=0) {
			Line = line;
			Pos = pos;
		}

		//public override void Execute(Scope scope) => Eval(scope);

		//public abstract Operand Eval(Scope scope);

		public abstract override string ToString();
	}

}