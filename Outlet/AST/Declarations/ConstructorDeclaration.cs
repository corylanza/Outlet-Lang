using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;
using Decl = Outlet.AST.Declarator;

namespace Outlet.AST {
	public class ConstructorDeclaration : FunctionDeclaration {

		public ConstructorDeclaration(Decl decl, List<Decl> parameters, List<TypeParameter> typeParameters, Statement body) 
            : base(decl, parameters, typeParameters, body) 
        {
		}

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);
	}
}
