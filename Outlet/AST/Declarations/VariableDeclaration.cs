using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.AST {
	public class VariableDeclaration : Declaration {
		
		private Identifier ID;
		private Expression Initializer;

		public VariableDeclaration(Identifier id, Expression initializer) {
			ID = id;
			Initializer = initializer;
		}

		public override void Resolve() {
			throw new NotImplementedException();
		}

		public override void Execute(Scope block) {
			block.AddVariable(ID, Initializer?.Eval(block));
		}

		public override string ToString() {
			string s = "var " + ID.ToString();
			if (Initializer is null) return s;
			else return s + " = " + Initializer.ToString();
		}
	}
}
