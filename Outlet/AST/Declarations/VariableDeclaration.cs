using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.AST {
	public class VariableDeclaration : Declaration {

		public readonly Declarator Decl;
		public readonly Expression Initializer;

		public VariableDeclaration(Declarator decl, Expression initializer) {
			Name = decl.ID;
			Decl = decl;
			Initializer = initializer;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() {
			string s = Decl.ToString();
			if (Initializer is null) return s;
			else return s + " = " + Initializer.ToString();
		}
	}
}
