using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Deref : Expression {

		public readonly Expression Left;
		public readonly string Identifier;
		public bool ArrayLength = false;

		public Deref(Expression left, Expression right) {
			Left = left;
			if (right is Variable id) Identifier = id.Name;
            else if (right is Literal<int> tupleIdx) Identifier = tupleIdx.Value.ToString();
            else throw new OutletException("expected identifier following dereferencing " + left.ToString());
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Left.ToString() + "." + Identifier.ToString();
	}
}
