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

		public override void Resolve(Scope scope) {
            Condition.Resolve(scope);
			//if (Iftrue is Block || Iftrue is Expression)
			Iftrue.Resolve(scope);
			//else throw new OutletException("expected code block or expression after if statement");
			//if (!(Iftrue is null) && Iffalse is Block || Iffalse is Expression) 
			Iffalse?.Resolve(scope);
			//else throw new OutletException("expected code block or expression for else condition");
		}

		public override void Execute(Scope scope) {
			if (Condition.Eval(scope).Value is bool b && b) Iftrue.Execute(scope);
			else if (Iffalse != null) Iffalse.Execute(scope);
		}


		public override string ToString() => "if ...";
	}
}
