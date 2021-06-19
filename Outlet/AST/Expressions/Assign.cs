using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class LocalAssign : Expression {

		public readonly Variable Variable;
		public readonly Expression Right;

		public LocalAssign(Variable variable, Expression right) => (Variable, Right) = (variable, right);

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"{Variable} = {Right}";
	}

	public class MemberAssign : Expression
	{
		public readonly MemberAccess Member;
		public readonly Expression Right;

		public MemberAssign(MemberAccess member, Expression right) => (Member, Right) = (member, right);

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"{Member} = {Right}";
	}

	public class ArrayAssign : Expression
	{
		public readonly ArrayAccess ArrayIdx;
		public readonly Expression Right;

		public ArrayAssign(ArrayAccess arrayIdx, Expression right) => (ArrayIdx, Right) = (arrayIdx, right);

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"{ArrayIdx} = {Right}";
	}
}
