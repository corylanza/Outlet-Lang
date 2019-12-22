using Outlet.Types;
using System;
using Type = Outlet.Types.Type;

namespace Outlet.Operands
{
    public class TypeObject : Operand, ITyped
    {
        public readonly Type Encapsulated;

        public TypeObject(Type t)
        {
            Encapsulated = t;
        }

        public override Type GetOutletType() => Primitive.MetaType;

        public bool Is(ITyped t) => t == Primitive.MetaType || t.Equals(this);

        public bool Is(ITyped t, out int level)
        {
            level = t == Primitive.MetaType ? 1 : t.Equals(this) ? 1 : -1;
            return level > -1;
        }

        public override bool Equals(Operand b) => b is TypeObject other && Encapsulated.Equals(other.Encapsulated);

        public override string ToString() => "type(" + Encapsulated.ToString() + ")";
    }
}
