using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ClassDeclaration : Declaration {

		public readonly string SuperClass;
		public readonly List<Declaration> InstanceDecls;
		public readonly List<Declaration> StaticDecls;
		public readonly ConstructorDeclaration Constructor;

		public ClassDeclaration(string name, string superclass, ConstructorDeclaration constructor, List<Declaration> instance, List<Declaration> statics) {
			Name = name;
			SuperClass = superclass;
			InstanceDecls = instance;
			StaticDecls = statics;
			Constructor = constructor;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Name + "{}";
	}
}
