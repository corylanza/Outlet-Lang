using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.AST
{
    public class TupleAccess : Expression
    {
        public readonly Expression Left;
        public readonly int Member;

        public TupleAccess(Expression left, int right)
        {
            Left = left;
            Member = right;
        }

        public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{Left}.{Member}";
    }
}
