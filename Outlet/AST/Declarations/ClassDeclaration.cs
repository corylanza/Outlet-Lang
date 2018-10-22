using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ClassDeclaration : Declaration {

		private string Name;
		private List<Identifier> ArgNames;


		public ClassDeclaration(Identifier name, List<Identifier> argnames) {
			Name = name.Name;
			ArgNames = argnames;
		}

		public override void Execute(Scope block) {
			// adds static class to scope
			Class c = new Class(Name, ArgNames);
			block.Add(Name, c);
			// adds constructor to scope
			block.Add(Name, new Native((args) => new Instance(c, ArgNames.TupleZip(args.ToList()))));
		}

		public override void Resolve(Scope block) {
			throw new NotImplementedException();
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
