using System;
using Outlet.Operands;
using Outlet.Checking;
using System.Collections.Generic;
using Decls = System.Collections.Generic.Dictionary<string, Outlet.Types.Type>;
using System.Linq;

namespace Outlet.Types {

	public abstract class Class : Type {

		public readonly string Name;
		public readonly Class Parent;

		public Class(string name, Class parent) {
			Name = name;
			Parent = parent;
		}

		//public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override bool Is(ITyped t, out int level) {
            //if (t is UnionType ut) return ut.Is(this, out level);
			level = 0;
			if(Equals(t)) return true;
			if(Parent != null && Parent.Is(t, out int l)) {
				level = l + 1;
				return true;
			}
			level = -1;
			return false;
		}

        public override string ToString() => Name;
    }

    public class UserDefinedClass : Class, IDereferenceable {

		public readonly Action<Instance> Init;
        private readonly Dictionary<string, Field> Members;

        public UserDefinedClass(string name, Class parent, Dictionary<string, Field> members, Action<Instance> init) : base(name, parent)
        {
            Members = members;
            Init = init;
        }

		public Operand GetMember(string id) => Members[id].Value;
		public void SetMember(string id, Operand value) => Members[id] = new Field { Value = value };

        public IEnumerable<(string id, Operand val)> GetMembers() => Members.Select(member => (member.Key, member.Value.Value));
    }

	public class ProtoClass : Class {

        public readonly Decls InstanceMembers;
        public readonly Decls StaticMembers;

        public ProtoClass(string name, Class parent, Decls statics, Decls instances) : base(name, parent) {
            InstanceMembers = instances;
            StaticMembers = statics;
		}

        public Type GetStaticMemberType(string s)
        {
            if (StaticMembers.ContainsKey(s)) return StaticMembers[s];
            if (s == "") return Checker.Error("type " + this + " is not instantiable");
            return Checker.Error(this + " does not contain static field: " + s);
        }
        public IEnumerable<(string id, Type type)> GetStaticMemberTypes() => StaticMembers.Select(member => (member.Key, member.Value));

        public Type GetInstanceMemberType(string s)
        {
            Class cur = this;
            while (cur != null)
            {
                if (cur is ProtoClass pc && pc.InstanceMembers.ContainsKey(s)) return pc.InstanceMembers[s];
                cur = cur.Parent;
            }
            return Checker.Error(this + " does not contain instance field: " + s);
        }
        public IEnumerable<(string id, Type type)> GetIntanceMemberTypes() => InstanceMembers.Select(member => (member.Key, member.Value));
    }

    public class GenericClass : Class
    {

        public readonly Type[] GenericArguments;

        public GenericClass(Class encapsulated, params Type[] genericArguments) : base(encapsulated.Name, encapsulated)
        {
            GenericArguments = genericArguments;
        }
    }
}
