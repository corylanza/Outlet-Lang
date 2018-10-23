using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.AST {
	public class VariableDeclaration : Declaration {
		
		private string ID;
		private Expression Initializer;

		public VariableDeclaration(Identifier id, Expression initializer) {
			ID = id.Name;
			Initializer = initializer;
		}

		public override void Resolve(Scope scope) {
            scope.Declare(ID);
            Initializer?.Resolve(scope);
            scope.Define(ID);
		}

		public override void Execute(Scope scope) {
			scope.Add(ID, Initializer?.Eval(scope));
		}

		public override string ToString() {
			string s = "var " + ID.ToString();
			if (Initializer is null) return s;
			else return s + " = " + Initializer.ToString();
		}
	}
}
