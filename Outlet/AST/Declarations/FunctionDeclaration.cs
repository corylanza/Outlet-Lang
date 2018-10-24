using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public class FunctionDeclaration : Declaration {
		
        public readonly string ID;
		private readonly List<Identifier> ArgNames;
		private readonly Statement Body;
		//private readonly Function Func;

		public FunctionDeclaration(Identifier id, List<Identifier> argnames, Statement body) {
            ID = id.Name;
			ArgNames = argnames;
			Body = body;
		}

		public Function Construct(Scope closure) => new Function(closure, ID, ArgNames, Body);

		public override void Resolve(Scope scope) {
			scope.Define(ID);
			Scope exec = new Scope(scope);
			foreach (Identifier arg in ArgNames) {
				exec.Define(arg.Name);
			}
			Body.Resolve(exec);
		}

		public override void Execute(Scope scope) {
            scope.Add(ID, Construct(scope));
        }

        public override string ToString() {
			string s = "func "+ID+"(";
			return s+")";
        }
    }
}
