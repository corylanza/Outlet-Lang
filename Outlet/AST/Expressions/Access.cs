using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Access : Expression {

		public Expression Collection;
		public Expression[] Index;

		public Access(Expression collection, Expression[] idx) {
			Collection = collection;
			Index = idx;
		}

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"{Collection}[{string.Join(",", Index.Select(x => x.ToString()))}]";
	}
}
