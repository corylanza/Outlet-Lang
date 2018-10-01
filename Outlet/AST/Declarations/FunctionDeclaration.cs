using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public class FunctionDeclaration : Declaration {
		
        private Identifier ID;
        private List<Identifier> ArgNames;
        private Statement Body;

        public FunctionDeclaration(Identifier id, List<Identifier> argnames, Statement body) {
            ID = id;
            ArgNames = argnames;
            Body = body;
        }

		public override void Resolve() {
			throw new NotImplementedException();
		}

		public override void Execute(Scope block) {
            Function f = new Function(ID, ArgNames, Body);
            block.AddVariable(ID, f);
        }

        public override string ToString() {
            throw new NotImplementedException();
        }
    }
}
