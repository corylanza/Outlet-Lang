using Outlet.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.Operands
{
    public class String : Operand<Primitive>
    {
        public override Primitive RuntimeType => Primitive.String;
        public readonly string Underlying;

        public String(string value) => Underlying = value;

        public override bool Equals(Operand b) => b is String other && other.Underlying.Equals(Underlying);

        public override string ToString() => Underlying;
    }
}
