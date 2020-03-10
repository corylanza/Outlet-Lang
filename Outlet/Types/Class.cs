using System;
using Outlet.Operands;
using Outlet.Checking;
using System.Collections.Generic;
using System.Linq;

namespace Outlet.Types {

	public abstract class Class : Type {

		public readonly string Name;
		public readonly Class? Parent;
        public const int This = 0;

		public Class(string name, Class? parent) {
			Name = name;
			Parent = parent;
		}

		//public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override bool Is(Type t, out uint level) {
            //if (t is UnionType ut) return ut.Is(this, out level);
			level = 0;
			if(Equals(t)) return true;
			if(Parent != null && Parent.Is(t, out uint l)) {
				level = l + 1;
				return true;
			}
			level = 0;
			return false;
		}

        public override string ToString() => Name;
    }

    public class UserDefinedClass : Class, IDereferenceable {

		private readonly Func<UserDefinedClass, (Instance, IStackFrame<Operand>)> Init;
        private readonly IStackFrame<Operand> StaticStackFrame;
        private readonly Field[] StaticMembers;

        public UserDefinedClass(string name, Class parent, IStackFrame<Operand> stackFrame, 
            Field[] staticMembers, Func<UserDefinedClass, (Instance, IStackFrame<Operand>)> init) : base(name, parent)
        {
            StaticMembers = staticMembers;
            StaticStackFrame = stackFrame;
            Init = init;
        }

        public (Instance, IStackFrame<Operand>) Initialize() => Init(this);

        public Operand GetMember(IBindable field) => StaticStackFrame.Get(field); //StaticMembers[id.LocalId].Value;
        public void SetMember(IBindable field, Operand value) => StaticStackFrame.Assign(field, value); // StaticMembers[id.LocalId] = new Field(id.Identifier, value);
        public IEnumerable<(string id, Operand val)> GetMembers() => StaticStackFrame.List();//StaticMembers.Select(field => (field.Name, field.Value));
    }

	public class ProtoClass : Class {

        public readonly CheckStackFrame InstanceMembers;
        public readonly CheckStackFrame StaticMembers;

        public ProtoClass(string name, Class? parent, CheckStackFrame statics, CheckStackFrame instances) : base(name, parent) {
            InstanceMembers = instances;
            StaticMembers = statics;
		}

        public Type GetStaticMemberType(IBindable variable)
        {
            if (StaticMembers.Resolve(variable, out Type type, out uint resolveLevel, out uint id)) {
                variable.Bind(id, resolveLevel);
                return type;
            }
            else if(variable.Identifier == "") return new Checker.Error("type " + this + " is not instantiable");
            else return new Checker.Error(this + " does not contain static field: " + variable.Identifier);
        }
        public IEnumerable<(string id, Type type)> GetStaticMemberTypes() => StaticMembers.List().Select(member => (member.Id, member.Value as Type));

        public Type GetInstanceMemberType(IBindable variable)
        {
            if (variable.Identifier == "this") return new Checker.Error("may not access property \"this\"");
            if (InstanceMembers.Resolve(variable, out Type type, out uint resolveLevel, out uint id))
            {
                variable.Bind(id, resolveLevel);
                return type;
            }
            return new Checker.Error(this + " does not contain instance field: " + variable.Identifier);

            //Class cur = this;
            //while (cur != null)
            //{
            //    if (cur is ProtoClass pc && pc.InstanceMembers.Has(variable.Identifier))
            //    {
            //        (Type type, int resolveLevel, int id) = pc.InstanceMembers.Resolve(variable);
            //        if(resolveLevel == 0) return Checker.Error(this + " does not contain instance field: " + variable.Identifier);
            //        return type as Type;
            //    }
            //    cur = cur.Parent;
            //}
            //return Checker.Error(this + " does not contain instance field: " + variable.Identifier);
        }
        public IEnumerable<(string id, Type type)> GetIntanceMemberTypes() => InstanceMembers.List().Select(member => (member.Id, member.Value as Type));
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
