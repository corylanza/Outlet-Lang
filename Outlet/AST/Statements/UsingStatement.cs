using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.AST
{
    public class UsingStatement : Statement
    {
        public Expression Used;

        public UsingStatement(Expression usedClass)
        {
            Used = usedClass;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "using " + Used;
        }
    }
}
