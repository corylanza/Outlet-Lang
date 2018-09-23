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

		public override void Execute() {
			while (Condition.Eval().Value is bool b && b) Iftrue.Execute();
		}

		public override string ToString() => "if ...";

	}
}
