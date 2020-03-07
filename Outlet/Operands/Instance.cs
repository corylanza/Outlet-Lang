using Outlet.Types;
using System.Collections.Generic;
using System.Linq;

namespace Outlet.Operands {
	public abstract class Instance : Operand<Class>, IDereferenceable
    {
        public override Class RuntimeType { get; set; }

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
        private readonly IStackFrame<Operand> InstanceStackFrame; 

        public UserDefinedInstance(Class type, IStackFrame<Operand> stackFrame, int instanceVarCount) : base(type) 
        {
            InstanceStackFrame = stackFrame;
            InstanceMembers = new Field[instanceVarCount];
        }

        // Subtract one from local id for getting and setting as local id 0 is reserved for "this"
        public override Operand GetMember(IBindable field) => InstanceStackFrame.Get(field); //InstanceMembers[field.LocalId].Value;
        // TODO this is inconsistent, should not need to subtract 1 from local id
        public override void SetMember(IBindable field, Operand value) => InstanceStackFrame.Assign(field, value);//InstanceMembers[field.LocalId - 1] = new Field(field.Identifier, value);
        public override IEnumerable<(string id, Operand val)> GetMembers() => InstanceStackFrame.List();//InstanceMembers.Select(member => (member?.Name, member?.Value));
    }
}
