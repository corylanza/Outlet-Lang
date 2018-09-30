using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.AST {
	public class VariableDeclaration : Declaration {

		private Scope scope;
		private Identifier ID;
		private Expression Initializer;

		public VariableDeclaration(Scope s, Identifier id, Expression initializer) {
			scope = s;
			ID = id;
			Initializer = initializer;
		}

		public override void Execute() {
			scope.AddVariable(ID, Initializer?.Eval());
		}

		public override string ToString() {
			string s = "var " + ID.ToString();
			if (Initializer is null) return s;
			else return s + " = " + Initializer.ToString();
		}
	}
}
