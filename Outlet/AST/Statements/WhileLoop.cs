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

		public override void Resolve(Scope scope) {
            Scope exec = new Scope(scope);
			Condition.Resolve(exec);
			Iftrue.Resolve(exec);
		}

		public override void Execute(Scope scope) {
            Scope exec = new Scope(scope);
            while (Condition.Eval(exec).Value is bool b && b) {
                Iftrue.Execute(exec);
                exec = new Scope(scope);
			}
		}

		public override string ToString() => "if ...";

	}
}
