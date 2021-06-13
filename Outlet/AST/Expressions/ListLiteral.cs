using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ListLiteral : Expression {

		public readonly Expression[] Args;

		public ListLiteral(params Expression[] vals) {
			Args = vals;
		}

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"[{string.Join(",", Args.Select(x => x.ToString()))}]";
	}
}
