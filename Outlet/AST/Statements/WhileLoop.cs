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

		public override void Resolve(Scope block) {
            Scope exec = new Scope(block);
			Condition.Resolve(exec);
			Iftrue.Resolve(exec);
		}

		public override void Execute(Scope block) {
            Scope exec = new Scope(block);
            while (Condition.Eval(exec).Value is bool b && b) {
                Iftrue.Execute(exec);
                exec = new Scope(block);
			}
		}

		public override string ToString() => "if ...";

	}
}
