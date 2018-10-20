using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public class FunctionDeclaration : Declaration {
		
        private string ID;
        private List<Identifier> ArgNames;
        private Statement Body;
		private Scope Closure;

        public FunctionDeclaration(Scope closure, Identifier id, List<Identifier> argnames, Statement body) {
            ID = id.Name;
            ArgNames = argnames;
            Body = body;
			Closure = closure;
        }

		public override void Resolve(Scope block) {
            Scope exec = new Scope(Closure);
            foreach(Identifier arg in ArgNames) {
                exec.Declare(arg.Name);
                exec.Define(arg.Name);
            }
            Body.Resolve(exec);
        }

		public override void Execute(Scope block) {
            Function f = new Function(Closure, ID, ArgNames, Body);
            block.AddFunc(ID, f);
        }

        public override string ToString() {
			string s = "func "+ID+"(";
			return s+")";
        }
    }
}
