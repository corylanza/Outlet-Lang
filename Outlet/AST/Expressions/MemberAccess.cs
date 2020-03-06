using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST 
{
    public class MemberAccess : Expression
    {
        public bool ArrayLength = false;
        public readonly Expression Left;
        public readonly Variable Member;

        public MemberAccess(Expression left, Variable right)
        {
            Left = left;
            Member = right;
        }

        public bool ArrayLengthAccess() => ArrayLength = Member.Identifier == "length";

        public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{Left}.{Member}";
    }
}
