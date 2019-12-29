using Outlet.Checking;
using Outlet.FFI.Natives;
using Outlet.Interpreting;
using Outlet.Operands;
using Outlet.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Outlet.FFI
{
    public static class NativeInitializer
    {
        public static Operand ToOutletOperand(object o)
        {
            Types.Type type = o is null ? null : Conversions.OutletType.GetValueOrDefault(o.GetType());
            return type switch {
                Primitive p when p == Primitive.String => Constant.String((string)o),
                Primitive p when p == Primitive.Int => Constant.Int((int)o),
                Primitive p when p == Primitive.Bool => Constant.Bool((bool)o),
                Primitive p when p == Primitive.Float => Constant.Float((float)o),
                Class nc => ToOutletInstance(nc, o),
                null => Constant.Null(),
                _ => throw new Exception("Cannot map type")
            };
        }

        public static Operand ToOutletInstance(Class nc, object o)
        {
            Operand Get(string id) => ToOutletOperand(o.GetType().GetField(id).GetValue(o));
            void Set(string id, Operand val) => o.GetType().GetField(id).SetValue(o, ToCSharpOperand(val));
            IEnumerable<(string id, Operand val)> List() => o.GetType().GetFields().Select(field => (field.Name, ToOutletOperand(field.GetValue(o))));
            return new Instance(nc, Get, Set, List);
        }

        public static RuntimeClass ToOutletClass(string name, Dictionary<string, MemberInfo> members)
        {

            Operand Get(string id) => members[id] switch
            {
                MethodInfo method => Convert(id, method),
                FieldInfo field => ToOutletOperand(field.DeclaringType.GetField(field.Name).GetValue(null)),
                ConstructorInfo constructor => Convert(constructor),
                _ => throw new NotImplementedException()
            };

            void Set(string id, Operand val)
            {
                if(members[id] is FieldInfo field) field.DeclaringType.GetField(field.Name).SetValue(null, ToCSharpOperand(val));
            }
            IEnumerable<(string id, Operand val)> List()
            {
                foreach(var name in members.Keys)
                {
                    yield return (name, Get(name));
                }
            }
            static void Init() { }

            return new RuntimeClass(name, Primitive.Object, Get, Set, List, Init);
        }

        public static object ToCSharpOperand(Operand o)
        {
            // TODO maybe make this an abstract method for operands
            return o switch
            {
                Constant c => c.GetValue(),
                Operands.Array a => a.Values().Select(val => ToCSharpOperand(val)),
                _ => throw new NotImplementedException()
            };
        }

        public static Types.Type Convert(System.Type input)
        {
            if (input.IsArray) return new ArrayType(Convert(input.GetElementType()));
            if(Conversions.OutletType.ContainsKey(input))
                return Conversions.OutletType[input];
            return Primitive.Object;
        }

        public static FunctionType ToOutletMethodType(MethodInfo method) =>
            new FunctionType(method.GetParameters()
                .Select(param => (Convert(param.ParameterType) as ITyped, param.Name))
                .ToArray(), Convert(method.ReturnType));

        public static NativeFunction Convert(string name, MethodInfo method) =>
            new NativeFunction(name, ToOutletMethodType(method), method);

        public static FunctionType ToOutletConstructorType(ConstructorInfo constructor) =>
            new FunctionType(constructor.GetParameters()
                .Select(param => (Convert(param.ParameterType) as ITyped, param.Name))
                .ToArray(), Convert(constructor.DeclaringType));

        public static NativeConstructor Convert(ConstructorInfo constructor) =>
            new NativeConstructor("", ToOutletConstructorType(constructor), constructor);

        public static Operand Convert(FieldInfo field)
        {
            return ToOutletOperand(field.GetValue(null));
        }

        #region Reflection

        private static IEnumerable<System.Type> GetForeignClasses() => 
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && x.GetCustomAttributes(typeof(ForeignClass)).FirstOrDefault() != null);

        private static IEnumerable<MethodInfo> GetMethods(System.Type type) => 
            type.GetMethods().Where(x => x.GetCustomAttributes(typeof(ForeignFunction), false).FirstOrDefault() != null);

        private static IEnumerable<FieldInfo> GetFields(System.Type type) =>
            type.GetFields().Where(x => x.GetCustomAttributes(typeof(ForeignField), false).FirstOrDefault() != null);

        private static IEnumerable<ConstructorInfo> GetConstructors(System.Type type) =>
            type.GetConstructors().Where(x => x.GetCustomAttributes(typeof(ForeignConstructor)).FirstOrDefault() != null);

        #endregion

        public static void Register()
        {
            var classes = GetForeignClasses();
            foreach(var type in classes)
            {
                ForeignClass fc = (ForeignClass)type.GetCustomAttribute(typeof(ForeignClass));

                string className = !string.IsNullOrEmpty(fc.Name) ? fc.Name : type.Name;

                var staticMembers = new Dictionary<string, MemberInfo>();
                var staticTypes = new Dictionary<string, Types.Type>();
                var instanceMembers = new Dictionary<string, MemberInfo>();
                var instanceTypes = new Dictionary<string, Types.Type>();

                // Checktime
                // Add Create and define ProtoClass first, allowing members to reference the type they are declared in
                ProtoClass proto = new ProtoClass(className, Primitive.Object, staticTypes, instanceTypes);
                TypeObject CheckTime = new TypeObject(proto);
                Conversions.OutletType.Add(type, proto);
                SymbolTable.Global.Define(CheckTime, className);

                foreach (FieldInfo field in GetFields(type))
                {
                    ForeignField ff = ((ForeignField)field.GetCustomAttribute(typeof(ForeignField)));
                    string ffName = ff.Name ?? field.Name;
                    if (field.IsStatic)
                    {
                        staticMembers[ffName] = field;
                        staticTypes[ffName] = Convert(field.FieldType);
                    }
                    else
                    {
                        instanceMembers[ffName] = field;
                        instanceTypes[ffName] = Convert(field.FieldType);
                    }
                }

                foreach (MethodInfo method in GetMethods(type))
                {
                    ForeignFunction fm = ((ForeignFunction)method.GetCustomAttribute(typeof(ForeignFunction)));
                    string fmName = fm.Name ?? method.Name;
                    if (method.IsStatic)
                    {
                        staticMembers[fmName] = method;
                        staticTypes[fmName] = ToOutletMethodType(method);
                    }
                    else
                    {
                        instanceMembers[fmName] = method;
                        instanceTypes[fmName] = ToOutletMethodType(method);
                    }
                }

                foreach (ConstructorInfo constructor in GetConstructors(type))
                {
                    staticMembers[""] = constructor;
                    staticTypes[""] = ToOutletConstructorType(constructor);
                }


                // Runtime
                Class c = ToOutletClass(className, staticMembers);
                TypeObject runtime = new TypeObject(c);
                Scope.Global.Add(className, Primitive.MetaType, runtime);

                Conversions.OutletType[type] = c;
            };
        }
    }
}
