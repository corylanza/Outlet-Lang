using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ClassDeclaration : Declaration {

		public Expression? SuperClass { get; private init; }
		public List<Declaration> InstanceDecls { get; private init; }
		public List<Declaration> StaticDecls { get; private init; }
		public List<TypeParameter> TypeParameters { get; private init; }
        public List<ConstructorDeclaration> Constructors { get; private init; }

		public ClassDeclaration(
			string name, 
			Variable? superclass,
            List<TypeParameter> genericParams, 
            List<ConstructorDeclaration> constructors, 
            List<Declaration> instance, List<Declaration> statics) : base(new Declarator(new Variable(name), name))
        {
			SuperClass = superclass;
			InstanceDecls = instance;
			StaticDecls = statics;
			TypeParameters = genericParams;
			Constructors = constructors;
		}

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => Name + "{}";
	}
}
