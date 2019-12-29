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
        public readonly Type[] GenericArguments;

		public Class(string name, Class parent, params Type[] genericArguments) {
			Name = name;
			Parent = parent;
            GenericArguments = genericArguments;
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

    public class RuntimeClass : Class, IDereferenceable {

		private readonly Getter StaticGetter;
		private readonly Setter StaticSetter;
        private readonly Lister GetList;
		public readonly Action Init;

		public RuntimeClass(string name, Class parent, Getter get, Setter set, Lister list, Action init) : base(name, parent) {
			StaticGetter = get;
			StaticSetter = set;
            GetList = list;
            Init = init;
		}

		public Operand GetMember(string s) => StaticGetter(s);
		public void SetMember(string s, Operand val) => StaticSetter(s, val);
        public IEnumerable<(string id, Operand val)> GetMembers()
        {
            return GetList();
        }
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
}
