using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ClassDeclaration : Declaration {

		private readonly string Name;
		private readonly List<Identifier> ArgNames;
		private readonly List<Declaration> InstanceDecls;
		private readonly List<Declaration> StaticDecls;

		public ClassDeclaration(Identifier name, List<Identifier> argnames, List<Declaration> instance, List<Declaration> statics) {
			Name = name.Name;
			ArgNames = argnames;
			InstanceDecls = instance;
			StaticDecls = statics;
		}

		public override void Execute(Scope scope) {
			Class c = new Class(Name, scope, ArgNames, InstanceDecls, StaticDecls);
			scope.Add(Name, c);
		}

		public override void Resolve(Scope scope) {
			scope.Define(Name);
			Scope staticExec = new Scope(scope);
			foreach (Declaration d in StaticDecls) {
				d.Resolve(staticExec);
			}
			Scope instanceExec = new Scope(staticExec);
			foreach(Declaration d in InstanceDecls) {
				d.Resolve(instanceExec);
			}
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
