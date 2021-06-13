using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class WhileLoop : Statement 
    {
		public readonly Expression Condition;
		public readonly Statement Body;

		public WhileLoop(Expression condition, Statement body) 
        {
			Condition = condition;
			Body = body;
		}

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"while({Condition}) {Body}";

	}
}
