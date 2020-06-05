using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Lambda : Expression {

		public readonly Expression Left, Right;

		public Lambda(Expression l, Expression r) {
			Left = l;
			Right = r;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => $"{Left} => {Right}";
	}
}
