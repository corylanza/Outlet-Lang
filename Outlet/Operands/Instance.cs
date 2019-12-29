using Outlet.Types;
using System.Collections.Generic;

namespace Outlet.Operands {
	public class Instance : Operand<Class>, IDereferenceable
    {

		private readonly Getter GetInstanceVar;
		private readonly Setter SetInstanceVar;
        private readonly Lister GetInstanceVars;

		public Instance(Class type, Getter get, Setter set, Lister list) 
        {
			RuntimeType = type;
			GetInstanceVar = get;
			SetInstanceVar = set;
            GetInstanceVars = list;
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

        public Operand GetMember(string field) => GetInstanceVar(field);
        public void SetMember(string field, Operand value) => SetInstanceVar(field, value);
        public IEnumerable<(string id, Operand val)> GetMembers() => GetInstanceVars();

        public override string ToString() {
			string s = RuntimeType.Name + " {\n";
            foreach (var (id, val) in GetInstanceVars())
            {
                if(id != "this") s += "    \"" + id + "\": " + val.ToString() + "\n";
            }
            return s + "}";
		}
	}
}
