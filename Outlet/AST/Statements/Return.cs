﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ReturnStatement : Statement {

		private Expression E;

		public ReturnStatement(Expression e) {
			E = e;
		}

		public override void Execute() => throw new Return(E.Eval());

		public override string ToString() => "return " + E.ToString();
	}

	public class Return : OutletException {

		Operand Value;

		public Return(Operand o) {
			Value = o;
		}
	}
}
