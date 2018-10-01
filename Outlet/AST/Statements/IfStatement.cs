﻿using System;
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

		public override void Resolve() {
			Condition.Resolve();
			Iftrue.Resolve();
			if (Iffalse != null) Iffalse.Resolve();
		}

		public override void Execute(Scope block) {
			if (Condition.Eval(block).Value is bool b && b) Iftrue.Execute(block);
			else if (Iffalse != null) Iffalse.Execute(block);
		}


		public override string ToString() => "if ...";
	}
}