using System;
using System.Collections.Generic;
using Outlet.Types;

namespace Outlet.AST {
	public class FunctionDeclaration : Declaration {

		public List<Declarator> Parameters { get; private set; }
		public List<TypeParameter> TypeParameters { get; private set; }
		public Statement Body { get; private set; }

        public uint? LocalCount;

		public FunctionDeclaration(Declarator decl, List<Declarator> parameters, List<TypeParameter> typeParameters, Statement body) : base(decl) {
			TypeParameters = typeParameters;
			Parameters = parameters;
			Body = body;
            LocalCount = null;
		}

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() {
			string s = "func " + Decl.Identifier + "(";
			return s + ")";
		}
	}
}
