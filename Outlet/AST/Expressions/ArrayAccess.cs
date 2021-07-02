using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ArrayAccess : Expression {

		public Expression Collection { get; private init; }
		public Expression[] Index { get; private init; }

		public ArrayAccess(Expression collection, Expression[] idx) {
			Collection = collection;
			Index = idx;
		}

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"{Collection}[{string.Join(",", Index.Select(x => x.ToString()))}]";
	}
}
