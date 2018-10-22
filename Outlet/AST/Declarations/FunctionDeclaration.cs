using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public class FunctionDeclaration : Declaration {
		
        private readonly string ID;
		private readonly List<Identifier> ArgNames;
		private readonly Statement Body;
		//private readonly Function Func;

		public FunctionDeclaration(Scope closure, Identifier id, List<Identifier> argnames, Statement body) {
            ID = id.Name;
			ArgNames = argnames;
			Body = body;
			//Func = new Function(closure, ID, argnames, body);
		}

		public override void Resolve(Scope block) {
			block.Declare(ID);
			block.Define(ID);
			Scope exec = new Scope(block);
			foreach (Identifier arg in ArgNames) {
				exec.Declare(arg.Name);
				exec.Define(arg.Name);
			}
			Body.Resolve(exec);
		}

		public override void Execute(Scope block) {
            block.Add(ID, new Function(block, ID, ArgNames, Body));
        }

        public override string ToString() {
			string s = "func "+ID+"(";
			return s+")";
        }
    }
}
