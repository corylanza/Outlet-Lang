using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ConstructorDeclaration : FunctionDeclaration {

		public ConstructorDeclaration(Declarator decl, List<Declarator> argnames, Statement body) : base(decl, argnames, body) {

		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}
	}
}
