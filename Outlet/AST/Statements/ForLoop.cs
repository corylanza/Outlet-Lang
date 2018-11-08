using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ForLoop : Statement {

		private readonly Declarator LoopVar;
		private readonly Expression Collection;
		private readonly Statement Body;

		public ForLoop(Declarator loopvar, Expression collection, Statement body) {
			LoopVar = loopvar;
			Collection = collection;
            Body = body;
		}

		public override void Resolve(Scope scope) {
            Scope exec = new Scope(scope);
			LoopVar.Resolve(exec);
            exec.Define(LoopVar.GetType(scope), LoopVar.ID);
            Collection.Resolve(exec);
            Body.Resolve(exec);
		}

		public override void Execute(Scope scope) {
            Scope exec = new Scope(scope);
			Operand c = Collection.Eval(exec);
			Type looptype = LoopVar.GetType(exec);
			if (c is ICollection collect) {
				foreach (Operand o in collect.Values()) {
					o.Cast(looptype);
					exec.Add(LoopVar.ID, looptype, o);
					Body.Execute(exec);
					exec = new Scope(scope);
				}
			} else throw new OutletException("for loops can only loop over collections");
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "for(" + LoopVar + " in " + Collection.ToString() + ")" + Body.ToString();
	}
}
