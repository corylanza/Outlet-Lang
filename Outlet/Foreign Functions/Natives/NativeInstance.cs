﻿using Outlet.Operands;
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
        public readonly object Underlying;

        public NativeInstance(Class type, object o, Getter get, Setter set, Lister list) : base(type)
        {
            GetInstanceVar = get;
            SetInstanceVar = set;
            GetInstanceVars = list;
            Underlying = o;
        }

        public override bool Equals(Operand b) => ReferenceEquals(this, b);

        public override Operand GetMember(IBindable field) => GetInstanceVar(field);
        public override void SetMember(IBindable field, Operand value) => SetInstanceVar(field, value);
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
