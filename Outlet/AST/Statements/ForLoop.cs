using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ForLoop : Statement {

		private readonly string LoopVar;
		private readonly Expression Collection;
		private readonly Statement Body;

		public ForLoop(Identifier loopvar, Expression collection, Statement body) {
			LoopVar = loopvar.Name;
			Collection = collection;
            Body = body;
		}

		public override void Resolve(Scope scope) {
            Scope exec = new Scope(scope);
            exec.Define(LoopVar);
            Collection.Resolve(exec);
            Body.Resolve(exec);
		}

		public override void Execute(Scope scope) {
            Scope exec = new Scope(scope);
			Operand c = Collection.Eval(exec);
			if (c is ICollection collect) {
				foreach (Operand o in collect.Values()) {
					exec.Add(LoopVar, Primitive.Object, o);
					Body.Execute(exec);
					exec = new Scope(scope);
				}
			} else throw new OutletException("for loops can only loop over collections");
		}

		public override string ToString() => "for " + LoopVar + " in " + Collection.ToString() + Body.ToString();
	}
}
