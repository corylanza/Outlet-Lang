using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Assign : BinaryExpression {

		public Assign(Expression left, Expression right) : base(left, right) { }

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"{Left} = {Right}";
	}
}
