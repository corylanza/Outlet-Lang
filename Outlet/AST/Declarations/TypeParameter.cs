using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.AST
{
    public class TypeParameter : IBindable
    {
        public string Identifier { get; private set; }
        public Expression? Constraint { get; private set; }
        public uint? ResolveLevel { get; private set; }
        public uint? LocalId { get; private set; }

        public TypeParameter(Expression? constraint, string id)
        {
            Identifier = id;
            Constraint = constraint;
        }

        public override string ToString() => "";

        public void Bind(uint id, uint level)
        {
            LocalId = id;
            if (level != 0) throw new UnexpectedException("Cannot bind generic parameter to resolve level other than 0");
            ResolveLevel = 0;
        }
    }
}
