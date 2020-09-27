using Outlet.Operators;
using Outlet.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.AST
{
    public class OperatorOverloadDeclaration : FunctionDeclaration
    {
        public Operator Operator { get; set; }

		public OperatorOverloadDeclaration(Declarator decl, OperatorToken op, List<Declarator> argnames, Statement body) : base(decl, argnames, body)
		{
			Operator = op.HasBinaryOperation(out var bin) ? bin : throw new NotImplementedException();
			LocalCount = null;
		}

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"operator {Operator} ()";
	}
}
