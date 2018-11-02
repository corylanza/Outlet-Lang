using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class WhileLoop : Statement {
		private readonly Expression Condition;
		private readonly Statement Body;

		public WhileLoop(Expression condition, Statement body) {
			Condition = condition;
			Body = body;
		}

		public override void Resolve(Scope scope) {
            Scope exec = new Scope(scope);
			Condition.Resolve(exec);
			Body.Resolve(exec);
		}

		public override void Execute(Scope scope) {
            Scope exec = new Scope(scope);
            while (Condition.Eval(exec).Value is bool b && b) {
                Body.Execute(exec);
                exec = new Scope(scope);
			}
		}

		public override string ToString() => "while("+Condition.ToString()+") "+Body.ToString();

	}
}
