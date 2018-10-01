using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ForLoop : Statement{

		private Identifier LoopVar;
		private Expression Collection;
		private Scope Body;

		public ForLoop(Scope parent, Identifier loopvar, Expression collection, Statement body) {
			LoopVar = loopvar;
			Collection = collection;
			if(body is Scope s) {
				Body = s;
			} else {
				Body = new Scope(parent);
				Body.Lines.Add(body);
			}
		}

		public override void Resolve() {
			throw new NotImplementedException();
		}

		public override void Execute(Scope block) {
			OList c = Collection.Eval(block) as OList;
			Body.AddVariable(LoopVar, null);
			foreach(Operand o in c.Value) {
				Console.WriteLine(o.ToString());
				//Body.Get(LoopVar).Value = o.Value;
				//Body.Execute();
			}
		}

		public override string ToString() => "for " + LoopVar.Name + " in " + Collection.ToString() + Body.ToString();
	}
}
