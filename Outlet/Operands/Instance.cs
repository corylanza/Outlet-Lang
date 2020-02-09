using Outlet.Types;
using System.Collections.Generic;
using System.Linq;

namespace Outlet.Operands {
	public abstract class Instance : Operand<Class>, IDereferenceable
    {
		public Instance(Class type)
        {
			RuntimeType = type;
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);
        public abstract Operand GetMember(IBindable field);
        public abstract void SetMember(IBindable field, Operand value);
        public abstract IEnumerable<(string id, Operand val)> GetMembers();

        public override string ToString() {
			string s = RuntimeType.Name + " {\n";
            foreach (var (id, val) in GetMembers())
            {
                if(id != "this") s += "    \"" + id + "\": " + val?.ToString() + "\n";
            }
            return s + "}";
		}
	}

    public class UserDefinedInstance : Instance
    {
        private readonly Field[] InstanceMembers;

        public UserDefinedInstance(Class type, int instanceVarCount) : base(type) 
        {
            InstanceMembers = new Field[instanceVarCount];
        }

        public override Operand GetMember(IBindable field) => InstanceMembers[field.LocalId].Value;
        public override void SetMember(IBindable field, Operand value) => InstanceMembers[field.LocalId] = new Field() { Value = value };
        public override IEnumerable<(string id, Operand val)> GetMembers() => InstanceMembers.Select(member => (member?.Name, member?.Value));
    }
}
