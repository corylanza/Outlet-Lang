using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Outlet.Operands;
using Outlet.Types;

namespace Outlet.FFI.Natives
{
    public class NativeClass : Class, IDereferenceable
    {
        public Dictionary<string, MemberInfo> InstanceMembers { get; private set; }
        private readonly Getter StaticGetter;
        private readonly Setter StaticSetter;
        private readonly Lister GetList;

        public NativeClass(string name, Class parent, Getter get, Setter set, Lister list, Dictionary<string, MemberInfo> instanceMembers) : base(name, parent)
        {
            StaticGetter = get;
            StaticSetter = set;
            GetList = list;
            InstanceMembers = instanceMembers;
        }

        public Operand GetMember(string s) => StaticGetter(s);
        public void SetMember(string s, Operand val) => StaticSetter(s, val);
        public IEnumerable<(string id, Operand val)> GetMembers() => GetList();
    }
}
