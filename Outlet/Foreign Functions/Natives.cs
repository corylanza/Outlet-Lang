using Outlet.Checking;
using Outlet.Operands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Outlet.FFI
{
    public class NativeFunction : Function
    {
        private readonly MethodInfo Underlying;

        public NativeFunction(string name, FunctionType type, MethodInfo func) : base(name, type)
        {
            Underlying = func;
        }

        public override Operand Call(params Operand[] args) =>
            FFIConfig.FromNative(Underlying.Invoke(null, args.Select(arg => FFIConfig.ToNative(arg)).ToArray()));
    }

    public class NativeClass : Class, IRuntimeClass, ICheckableClass
    {
        private readonly Dictionary<string, MemberInfo> StaticMembers = new Dictionary<string, MemberInfo>();
        public Action Init;

        public NativeClass(string name, Class parent, IEnumerable<MemberInfo> members) 
            : base(name, parent, null)
        {
            // TODO filter for only static members
            StaticMembers = members.ToDictionary(member => member switch {
                FieldInfo field => ((ForeignField)field.GetCustomAttribute(typeof(ForeignField))).Name ?? field.Name,
                MethodInfo method => ((ForeignFunction)method.GetCustomAttribute(typeof(ForeignFunction))).Name ?? method.Name,
                ConstructorInfo constructor => "",
                _ => throw new Exception("")
            });
        }

        public void RegisterMembers()
        {
            /*foreach (MethodInfo method in Methods)
            {
                ForeignFunction fm = (ForeignFunction)method.GetCustomAttribute(typeof(ForeignFunction));
                string name = !string.IsNullOrEmpty(fm.Name) ? fm.Name : method.Name;
                NativeFunction nativeMethod = FFIConfig.Convert(name, method);

                if (method.IsStatic) StaticMembers[name] = nativeMethod;
            }
            foreach (FieldInfo field in Fields)
            {
                ForeignField fm = (ForeignField)field.GetCustomAttribute(typeof(ForeignField));
                string name = !string.IsNullOrEmpty(fm.Name) ? fm.Name : field.Name;
                //Operand nativefield = FFIConfig.Convert(field);
                // TODO to allow fields to be mutated by foreign code, must store fieldinfo not operand

                if (field.IsStatic) StaticMembers[name] = field;// nativefield;
            }
            foreach (ConstructorInfo constructor in Constructors)
            {
                // TODO This should probably handle instance vars
                // Constructor cannot be overloaded as function overloads not supported
                // StaticMembers[""] = constructor.
                // new NativeFunction(Name, 
                // new Instance(this, (id) => new Dictionary<string, Operand>().GetValueOrDefault(id), null);
            }*/
        }

        public Operand GetStatic(string s) => StaticMembers.ContainsKey(s) ? StaticMembers[s] switch
        {
            FieldInfo field => FFIConfig.Convert(field),
            MethodInfo method => FFIConfig.Convert(s, method),
            _ => throw new Exception("")
        } 
        : throw new OutletException("Type " + Name + " does not contain static member " + s);

        public void SetStatic(string s, Operand val)
        {
            if (StaticMembers.ContainsKey(s))
            {
                switch(StaticMembers[s])
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
            }throw new OutletException("Type " + Name + " does not contain static member " + s);
        }
        public Operands.Type GetStaticType(string s) => StaticMembers.ContainsKey(s) ? StaticMembers[s] switch 
        {
            FieldInfo field => FFIConfig.Convert(field.FieldType),
            MethodInfo method => FFIConfig.Convert(s, method).Type,
            _ => throw new Exception("")
        } 
        : (s == "") ? Checker.Error("type " + this + " is not instantiable")
        : Checker.Error(this + " does not contain static field: " + s);
        
        public Operands.Type GetInstanceType(string s) => throw new NotImplementedException();
    }
}
