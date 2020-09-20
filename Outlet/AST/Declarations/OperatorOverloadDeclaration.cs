﻿using Outlet.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.AST
{
    public class OperatorOverloadDeclaration : FunctionDeclaration
    {
        public Operator Operator { get; set; }

		public OperatorOverloadDeclaration(Declarator decl, Operator op, List<Declarator> argnames, Statement body) : base(decl, argnames, body)
		{
			Operator = op;
			LocalCount = null;
		}

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"operator {Operator} {Decl.Identifier} ()";
	}
}
