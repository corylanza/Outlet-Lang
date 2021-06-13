using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Lambda : BinaryExpression
	{
		public Lambda(Expression l, Expression r) : base(l, r) { }

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"{Left} => {Right}";
	}
}
