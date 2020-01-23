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
        public abstract Operand GetMember(string field);
        public abstract void SetMember(string field, Operand value);
        public abstract IEnumerable<(string id, Operand val)> GetMembers();

        public override string ToString() {
			string s = RuntimeType.Name + " {\n";
            foreach (var (id, val) in GetMembers())
            {
                if(id != "this") s += "    \"" + id + "\": " + val.ToString() + "\n";
            }
            return s + "}";
		}
	}

    public class UserDefinedInstance : Instance
    {
        private readonly Dictionary<string, Field> Members = new Dictionary<string, Field>();

        public UserDefinedInstance(Class type) : base(type) { }

        public override Operand GetMember(string field) => Members[field].Value;
        public override void SetMember(string field, Operand value) => Members[field] = new Field() { Value = value };
        public override IEnumerable<(string id, Operand val)> GetMembers() => Members.Select(member => (member.Key, member.Value.Value));
    }
}
