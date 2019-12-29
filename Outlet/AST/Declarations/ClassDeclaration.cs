using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ClassDeclaration : Declaration {

		public readonly Variable SuperClass;
		public readonly List<Declaration> InstanceDecls;
		public readonly List<Declaration> StaticDecls;
        public readonly List<(string id, Variable classConstraint)> GenericParameters;
        public readonly ConstructorDeclaration Constructor;

		public ClassDeclaration
            (string name, Variable superclass,
            List<(string id, Variable classConstraint)> genericParams, ConstructorDeclaration constructor, 
            List<Declaration> instance, List<Declaration> statics) 
        {
			Name = name;
			SuperClass = superclass;
			InstanceDecls = instance;
			StaticDecls = statics;
            GenericParameters = genericParams;
			Constructor = constructor;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Name + "{}";
	}
}
