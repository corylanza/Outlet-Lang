using Outlet.Checking;
using Outlet.Types;
using System.Collections.Generic;
using System.Linq;
using Type = Outlet.Types.Type;

namespace Outlet.Operands
{
    public class TypeObject : Operand, ITyped, ICallable, IDereferenceable
    {
        public readonly Type Encapsulated;

        public TypeObject(Type t)
        {
            Encapsulated = t;
        }

        #region Run time
        public Operand GetMember(string field) => (Encapsulated as IDereferenceable).GetMember(field);
        public void SetMember(string field, Operand value) => (Encapsulated as IDereferenceable).SetMember(field, value);
        public IEnumerable<(string id, Operand val)> GetMembers() => (Encapsulated as IDereferenceable).GetMembers();

        public Operand Call(params Operand[] args)
        {
            if (Encapsulated is IDereferenceable rc && rc.GetMember("") is ICallable callable) return callable.Call(args);
            throw new RuntimeException("Attempted to call something other than a function at runtime");
        }

        #endregion

        public override Type GetOutletType() => Primitive.MetaType;
        public bool Is(ITyped t) => t == Primitive.MetaType || t.Equals(this);
        public bool Is(ITyped t, out int level)
        {
            level = t == Primitive.MetaType ? 1 : t.Equals(this) ? 1 : -1;
            return level > -1;
        }

        public override bool Equals(Operand b) => b is TypeObject other && Encapsulated.Equals(other.Encapsulated);
        public override string ToString()
        {
            string s = "type(";
            if(Encapsulated is ProtoClass p) return s += p.Name + ")";
            else if(Encapsulated is RuntimeClass r)
            {
                s+= r.Name + "{\n";
                foreach (var (name, value) in r.GetMembers())
                {
                    s += "    \"" + name + "\": " + value.ToString() + " \n";
                }
                return s + "}";
            }
            else return s + Encapsulated.ToString() + ")";
        }
    }
}
