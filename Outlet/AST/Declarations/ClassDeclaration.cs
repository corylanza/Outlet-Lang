using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ClassDeclaration : Declaration {

		private readonly string Name;
		private readonly List<Identifier> ArgNames;


		public ClassDeclaration(Identifier name, List<Identifier> argnames) {
			Name = name.Name;
			ArgNames = argnames;
		}

		public override void Execute(Scope scope) {
			// adds static class to scope
			Class c = new Class(Name, ArgNames);
			scope.Add(Name, c);
		}

		public override void Resolve(Scope scope) {
			scope.Define(Name);
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
