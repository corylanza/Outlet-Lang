using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ClassDeclaration : Declaration {

		private Identifier Name;
		private List<Identifier> ArgNames;


		public ClassDeclaration(Identifier name, List<Identifier> argnames) {
			Name = name;
			ArgNames = argnames;
		}

		public override void Execute(Scope block) {
			throw new NotImplementedException();
		}

		public override void Resolve() {
			throw new NotImplementedException();
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
