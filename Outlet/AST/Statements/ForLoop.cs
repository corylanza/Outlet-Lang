using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ForLoop : Statement{

		private string LoopVar;
		private Expression Collection;
		private Statement Body;

		public ForLoop(Identifier loopvar, Expression collection, Statement body) {
			LoopVar = loopvar.Name;
			Collection = collection;
            Body = body;
		}

		public override void Resolve(Scope scope) { 
            /*
            Scope exec = new Scope(block);
            exec.Declare(LoopVar);
            exec.Define(LoopVar);
            Collection.Resolve(exec);
            Body.Resolve(exec);
            */
		}

		public override void Execute(Scope scope) {
            throw new NotImplementedException("");
            /*
            Scope exec = new Scope(block);
            Scope body = new Scope(exec) { Lines = new List<Declaration>() { Body } };
            OList c = Collection.Eval(exec) as OList;
			foreach(Operand o in c.Value) {
                exec.AddVariable(LoopVar, o);
                Body.Execute(exec);
                exec.Variables.Clear();
			}*/
		}

		public override string ToString() => "for " + LoopVar + " in " + Collection.ToString() + Body.ToString();
	}
}
