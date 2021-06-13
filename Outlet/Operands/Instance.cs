using Outlet.Types;
using System.Collections.Generic;
using System.Linq;

namespace Outlet.Operands {
	public abstract class Instance : Operand<Class>, IDereferenceable
    {
        public override Class RuntimeType { get; }

        protected Instance(Class type)
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
        private readonly IStackFrame<Operand> InstanceStackFrame; 

        public UserDefinedInstance(Class type, IStackFrame<Operand> stackFrame) : base(type) 
        {
            InstanceStackFrame = stackFrame;
        }

        public override Operand GetMember(IBindable field) => InstanceStackFrame.Get(field);
        public override void SetMember(IBindable field, Operand value) => InstanceStackFrame.Assign(field, value);
        public override IEnumerable<(string id, Operand val)> GetMembers() => InstanceStackFrame.List();
    }
}
