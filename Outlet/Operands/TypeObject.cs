using Outlet.Checking;
using Outlet.Types;
using System.Collections.Generic;
using System.Linq;
using Type = Outlet.Types.Type;

namespace Outlet.Operands
{
    public class TypeObject : Operand<MetaType>, IDereferenceable
    {
        public readonly Type Encapsulated;

        public TypeObject(Type t)
        {
            Encapsulated = t;
            RuntimeType = new MetaType(t);
        }

        #region Run time
        public Operand GetMember(IBindable field) => (Encapsulated as IDereferenceable).GetMember(field);
        public void SetMember(IBindable field, Operand value) => (Encapsulated as IDereferenceable).SetMember(field, value);
        public IEnumerable<(string id, Operand val)> GetMembers() => (Encapsulated as IDereferenceable).GetMembers();

        #endregion

        public override Type GetOutletType() => Primitive.MetaType;
        public bool Is(Type t) => t == Primitive.MetaType || t.Equals(this);
        public bool Is(Type t, out int level)
        {
            level = t == Primitive.MetaType ? 1 : t.Equals(this) ? 1 : -1;
            return level > -1;
        }

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
