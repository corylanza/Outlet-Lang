﻿using System;
using System.Collections.Generic;
using Outlet.Operands;
using System.Reflection;
using Outlet.Checking;

namespace Outlet.FFI.Natives
{
    public class NativeClass : Class, IRuntimeClass, ICheckableClass
    {
        private readonly Dictionary<string, MemberInfo> StaticMembers = new Dictionary<string, MemberInfo>();
        private readonly Dictionary<string, MemberInfo> InstanceMembers = new Dictionary<string, MemberInfo>();

        public NativeClass(string name, Class parent, IEnumerable<MemberInfo> members)
            : base(name, parent, null)
        {
            foreach (var member in members)
            {
                switch (member)
                {
                    case FieldInfo field:
                        ForeignField ff = ((ForeignField)field.GetCustomAttribute(typeof(ForeignField)));
                        string ffName = ff.Name ?? field.Name;
                        if (field.IsStatic) StaticMembers[ffName] = field;
                        else InstanceMembers[ffName] = field;
                        break;
                    case MethodInfo method:
                        ForeignFunction fm = ((ForeignFunction)method.GetCustomAttribute(typeof(ForeignFunction)));
                        string fmName = fm.Name ?? method.Name;
                        if (method.IsStatic) StaticMembers[fmName] = method;
                        else InstanceMembers[fmName] = method;
                        break;
                    case ConstructorInfo constructor:
                        StaticMembers[""] = constructor;
                        break;
                }
            }
        }

        private static Field ToField(FieldInfo field)
        {
            return new Field
            {
                Value = FFIConfig.FromNative(field.GetValue(null)),
                IsConstant = field.IsLiteral && !field.IsInitOnly,
                IsPublic = field.IsPublic,
                IsReadonly = field.IsInitOnly
            };
        }

        public void RegisterMembers()
        {
        }

        public Operand GetStatic(string s) => StaticMembers.ContainsKey(s) ? StaticMembers[s] switch
        {
            FieldInfo field => FFIConfig.Convert(field),
            MethodInfo method => FFIConfig.Convert(s, method),
            ConstructorInfo constructor => FFIConfig.Convert(constructor),
            _ => throw new Exception("")
        }
        : throw new OutletException("Type " + Name + " does not contain static member " + s);

        public void SetStatic(string s, Operand val)
        {
            if (StaticMembers.ContainsKey(s))
            {
                switch (StaticMembers[s])
                {
                    case FieldInfo f:
                        if (f.IsLiteral && !f.IsInitOnly) throw new RuntimeException("cannot assign to const");
                        f.SetValue(null, FFIConfig.ToNative(val));
                        return;
                    case MethodInfo m:
                        throw new OutletException("cannot assign to native method");
                    default:
                        throw new NotImplementedException();
                }
            }
            throw new OutletException("Type " + Name + " does not contain static member " + s);
        }
        public Operands.Type GetStaticType(string s) => StaticMembers.ContainsKey(s) ? StaticMembers[s] switch
        {
            FieldInfo field => FFIConfig.Convert(field.FieldType),
            MethodInfo method => FFIConfig.Convert(s, method).Type,
            ConstructorInfo constructor => FFIConfig.Convert(constructor).Type,
            _ => throw new Exception("")
        }
        : (s == "") ? Checker.Error("type " + this + " is not instantiable")
        : Checker.Error(this + " does not contain static field: " + s);

        public Operands.Type GetInstanceType(string s) => InstanceMembers.ContainsKey(s) ? InstanceMembers[s] switch
        {
            FieldInfo field => FFIConfig.Convert(field.FieldType),
            MethodInfo method => FFIConfig.Convert(s, method).Type,
            ConstructorInfo constructor => FFIConfig.Convert(constructor).Type,
            _ => throw new Exception("")
        }
        : Checker.Error(this + " does not contain instance field: " + s);
    }
}
