using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class WhileLoop : Statement {
		private Expression Condition;
		private Statement Iftrue;

		public WhileLoop(Expression condition, Statement iftrue) {
			Condition = condition;
			Iftrue = iftrue;
		}

		public override void Execute(Scope block) {
			while (Condition.Eval(block).Value is bool b && b) Iftrue.Execute(block);
		}

		public override string ToString() => "if ...";

	}
}
