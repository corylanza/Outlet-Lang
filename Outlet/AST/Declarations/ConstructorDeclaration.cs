using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;
using Decl = Outlet.AST.Declarator;

namespace Outlet.AST {
	public class ConstructorDeclaration : FunctionDeclaration {

		public ConstructorDeclaration(Decl decl, List<Decl> argnames, Statement body) : base(decl, argnames, body) {

		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}
	}
}
