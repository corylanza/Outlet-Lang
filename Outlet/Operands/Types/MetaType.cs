using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.Operands
{
    public class MetaType : Type
    {
        public Type HiddenType { get; private set; }

        public MetaType(Type t)
        {
            HiddenType = t;
        }

        public override bool Equals(Operand b) => b is MetaType other && HiddenType.Equals(other.HiddenType);
        public override bool Is(Type t, out int level) 
        {
            if (t is MetaType other && HiddenType.Is(other.HiddenType, out level))
                return true;
            level = t == Primitive.MetaType ? 1 : -1;
            return t == Primitive.MetaType;
        }
        public override string ToString() => "type(" + HiddenType + ")";
    }
}
