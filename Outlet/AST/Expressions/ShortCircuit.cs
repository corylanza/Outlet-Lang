using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operators;
using Outlet.Tokens;

namespace Outlet.AST {
	public abstract class ShortCircuit : BinaryExpression
	{
		public ShortCircuit(Expression left, Expression right) : base(left, right) { }

		public abstract bool IsAnd { get; }

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);
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
