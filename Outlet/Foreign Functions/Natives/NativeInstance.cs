using Outlet.Operands;
using Outlet.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.FFI.Natives
{
    public class NativeInstance : Instance
    {
        private readonly Getter GetInstanceVar;
        private readonly Setter SetInstanceVar;
        private readonly Lister GetInstanceVars;

        public NativeInstance(Class type, Getter get, Setter set, Lister list) : base(type)
        {
            GetInstanceVar = get;
            SetInstanceVar = set;
            GetInstanceVars = list;
        }

        public override bool Equals(Operand b) => ReferenceEquals(this, b);

        public override Operand GetMember(string field) => GetInstanceVar(field);
        public override void SetMember(string field, Operand value) => SetInstanceVar(field, value);
        public override IEnumerable<(string id, Operand val)> GetMembers() => GetInstanceVars();

        public override string ToString()
        {
            string s = RuntimeType.Name + " {\n";
            foreach (var (id, val) in GetInstanceVars())
            {
                if (id != "this") s += "    \"" + id + "\": " + val.ToString() + "\n";
            }
            return s + "}";
        }
    }
}
