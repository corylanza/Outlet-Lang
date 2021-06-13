using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ForLoop : Statement {

		public readonly Declarator LoopVar;
		public readonly Expression Collection;
		public readonly Statement Body;

		public ForLoop(Declarator loopvar, Expression collection, Statement body) {
			LoopVar = loopvar;
			Collection = collection;
            Body = body;
		}

		public override T Accept<T>(IASTVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "for(" + LoopVar + " in " + Collection.ToString() + ")" + Body.ToString();
	}
}
