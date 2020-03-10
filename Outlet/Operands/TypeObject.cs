using Outlet.Checking;
using Outlet.Types;
using System.Collections.Generic;
using System.Linq;
using Type = Outlet.Types.Type;

namespace Outlet.Operands
{
    public class TypeObject : Operand<MetaType>
    {
        public readonly Type Encapsulated;

        public override MetaType RuntimeType { get; set; }

        public TypeObject(Type t)
        {
            Encapsulated = t;
            RuntimeType = new MetaType(t);
        }

        public override Type GetOutletType() => Primitive.MetaType;

        public override bool Equals(Operand b) => b is TypeObject other && Encapsulated.Equals(other.Encapsulated);
        public override string ToString()
        {
            string s = "type(";
            if(Encapsulated is ProtoClass p) return s += p.Name + ")";
            else if(Encapsulated is Class c && c is IDereferenceable d)
            {
                s+= c.Name + "{\n";
                foreach (var (name, value) in d.GetMembers())
                {
                    s += "    \"" + name + "\": " + value?.ToString() + " \n";
                }
                return s + "}";
            }
            else return s + Encapsulated.ToString() + ")";
        }
    }
}
