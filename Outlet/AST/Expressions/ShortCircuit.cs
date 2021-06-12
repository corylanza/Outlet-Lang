using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operators;
using Outlet.Tokens;

namespace Outlet.AST {
	public abstract class ShortCircuit : Expression {

		public readonly Expression Left;
		public readonly Expression Right;

		public ShortCircuit(Expression left, Expression right) {
			Left = left;
			Right = right;
		}

		public abstract bool IsAnd { get; }

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}
	}

	public class LogicalAnd : ShortCircuit
    {
		public LogicalAnd(Expression left, Expression right) : base(left, right) { }

		public override bool IsAnd => true;

		public override string ToString() => $"({Left} && {Right})";
	}

	public class LogicalOr : ShortCircuit
	{
		public LogicalOr(Expression left, Expression right) : base(left, right) { }

		public override bool IsAnd => false;

		public override string ToString() => $"({Left} || {Right})";
	}
}
