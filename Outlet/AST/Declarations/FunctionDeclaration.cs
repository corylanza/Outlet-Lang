using System;
using System.Collections.Generic;
using Outlet.Types;

namespace Outlet.AST {
	public class FunctionDeclaration : Declaration {

		public readonly List<Declarator> Args;
		public readonly Statement Body;
		public FunctionType Type;   // set in the checking phase

		public FunctionDeclaration(Declarator decl, List<Declarator> argnames, Statement body) {
			Name = decl.Identifier;
			Decl = decl;
			Args = argnames;
			Body = body;
		}

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() {
			string s = "func " + Decl.Identifier + "(";
			return s + ")";
		}
	}
}
