using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Conditional : Statement {

		Expression Condition;
		Statement Iftrue;

		public Conditional(Expression condition, Statement iftrue) {
			Condition = condition;
			Iftrue = iftrue;
		}

		public override void Execute() {
			if (Condition.Eval().Value is bool b && b) Iftrue.Execute();
		}

		public override string ToString() => "if ...";
	}
}
