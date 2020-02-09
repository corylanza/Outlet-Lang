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
        // needs to be stored within class level despite holding instance members for the initializer
        public (string id, MemberInfo member)[] InstanceMembers { get; private set; }
        private readonly Getter StaticGetter;
        private readonly Setter StaticSetter;
        private readonly Lister GetList;

        public NativeClass(string name, Class parent, Getter get, Setter set, Lister list, (string, MemberInfo)[] instanceMembers) : base(name, parent)
        {
            StaticGetter = get;
            StaticSetter = set;
            GetList = list;
            InstanceMembers = instanceMembers;
        }

        public Operand GetMember(IBindable s) => StaticGetter(s);
        public void SetMember(IBindable s, Operand val) => StaticSetter(s, val);
        public IEnumerable<(string id, Operand val)> GetMembers() => GetList();
    }
}
