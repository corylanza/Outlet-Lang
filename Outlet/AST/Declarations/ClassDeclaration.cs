using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ClassDeclaration : Declaration {

		public readonly string Name;
		public readonly List<Declaration> InstanceDecls;
		public readonly List<Declaration> StaticDecls;

		public ClassDeclaration(string name, List<Declaration> instance, List<Declaration> statics) {
			Name = name;
			InstanceDecls = instance;
			StaticDecls = statics;
		}
		/*
		public override void Execute(Scope scope) {
			Class c = new Class(Name, scope, InstanceDecls, StaticDecls);
			scope.Add(Name, Primitive.MetaType, c);
		}

		public override void Resolve(Scope scope) {
			scope.Define(Primitive.MetaType, Name);
			Scope staticExec = new Scope(scope);
			foreach (Declaration d in StaticDecls) {
				d.Resolve(staticExec);
			}
			Scope instanceExec = new Scope(staticExec);
			instanceExec.Define(Primitive.MetaType, "this");
			foreach (Declaration d in InstanceDecls) {
				d.Resolve(instanceExec);
			}
		}
		*/
		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
