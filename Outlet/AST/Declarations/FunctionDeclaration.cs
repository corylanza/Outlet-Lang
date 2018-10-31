using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public class FunctionDeclaration : Declaration {
		
        private readonly Declarator Decl;
		private readonly List<Declarator> Args;
		private readonly Statement Body;

		public FunctionDeclaration(Declarator decl, List<Declarator> argnames, Statement body) {
			Decl = decl;
			Args = argnames;
			Body = body;
		}

		public Function Construct(Scope closure) {
			List<(Type, string)> args = Args.Select(x => (x.GetType(closure), x.ID)).ToList();
			return new Function(closure, Decl.ID, Decl.GetType(closure), args, Body);
		}

		public override void Resolve(Scope scope) {
			scope.Define(Decl.ID);
			Scope exec = new Scope(scope);
			Args.ForEach(x => exec.Define(x.ID));
			Body.Resolve(exec);
		}

		public override void Execute(Scope scope) {
            scope.Add(Decl.ID, null, Construct(scope));
        }

        public override string ToString() {
			string s = "func "+Decl.ID+"(";
			return s+")";
        }
    }
}
