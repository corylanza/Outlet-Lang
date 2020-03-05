using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class IfStatement : Statement {

		public readonly Expression Condition;
        public readonly Statement Iftrue;
        public readonly Statement? Iffalse;

		public IfStatement(Expression condition, Statement iftrue, Statement? ifelse) {
			Condition = condition;
			Iftrue = iftrue;
			Iffalse = ifelse;
		}
		/*
		public override void Resolve(Scope scope) {
            Condition.Resolve(scope);
			Iftrue.Resolve(scope);
			Iffalse?.Resolve(scope);
		}

		public override void Execute(Scope scope) {
			if (Condition.Eval(scope).Value is bool b && b) Iftrue.Execute(scope);
			else if (Iffalse != null) Iffalse.Execute(scope);
		}
		*/
		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "if("+Condition.ToString()+") "+Iftrue.ToString() + (Iffalse != null ? " else "+Iffalse.ToString() : "");
	}
}
