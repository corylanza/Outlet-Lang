using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class IfStatement : Statement {

		private Expression Condition;
		private Statement Iftrue, Iffalse;

		public IfStatement(Expression condition, Statement iftrue, Statement ifelse) {
			Condition = condition;
			Iftrue = iftrue;
			Iffalse = ifelse;
		}

		public override void Execute() {
			if (Condition.Eval().Value is bool b && b) Iftrue.Execute();
			else if (Iffalse != null) Iffalse.Execute();
		}

		public override string ToString() => "if ...";
	}
}
