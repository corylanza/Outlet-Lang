using Outlet.Checking;
using Outlet.FFI.Natives;
using Outlet.Operands;
using Outlet.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Outlet.FFI
{
    public static class NativeInitializer
    {

        public static Operand ToOutletOperand(object? o) => o switch
        {
            string s => new Operands.String(s),
            float f => Value.Float(f),
            bool b => Value.Bool(b),
            int i => Value.Int(i),
            null => Value.Null,
            IEnumerable collection =>
                new Operands.Array(collection.OfType<object>().Select(element => ToOutletOperand(element)).ToArray()),
            _ when Conversions.OutletType.ContainsKey(o.GetType()) => ToOutletInstance((Conversions.OutletType[o.GetType()] as NativeClass)!, o),
            _ => throw new UnexpectedException("Cannot map type")
        };

        public static Operand ToOutletInstance(NativeClass nc, object o)
        {
            static Operand ToMember(string id, MemberInfo member, object o) => member switch
            {
                MethodInfo method => Convert(id, method),
                FieldInfo field => ToOutletOperand(o.GetType()!.GetField(field.Name)!.GetValue(o)),
                _ => throw new NotSupportedException()
            };

            Operand Get(IBindable id)
            {
                var member = nc.InstanceMembers[id.LocalId.Value];
                return ToMember(member.id, member.member, o);
            }

            void Set(IBindable id, Operand val)
            {
                if (nc.InstanceMembers[id.LocalId.Value].member is FieldInfo field) field.DeclaringType!.GetField(field.Name)!.SetValue(o, ToCSharpOperand(val));
            }

            IEnumerable<(string id, Operand val)> List()
            {
                foreach ((string id, MemberInfo member) in nc.InstanceMembers)
                {
                    yield return (id, ToMember(id, member, o));
                }
            }

            return new NativeInstance(nc, o, Get, Set, List);
        }

        public static NativeClass ToOutletClass(string name, (string id, MemberInfo member)[] staticMembers, (string, MemberInfo)[] instanceMembers)
        {
            static Operand ToMember(string id, MemberInfo member) => member switch
            {
                MethodInfo method => Convert(id, method),
                FieldInfo field => ToOutletOperand(field.DeclaringType!.GetField(field.Name)!.GetValue(null)),
                ConstructorInfo constructor => Convert(constructor),
                _ => throw new NotSupportedException()
            };

            Operand Get(IBindable id)
            {
                var member = staticMembers[id.LocalId.Value];
                return ToMember(member.id, member.member);
            };

            void Set(IBindable id, Operand val)
            {
                if (staticMembers[id.LocalId.Value].member is FieldInfo field) field.DeclaringType!.GetField(field.Name)!.SetValue(null, ToCSharpOperand(val));
            }
            IEnumerable<(string id, Operand val)> List()
            {
                foreach ((string id, MemberInfo member) in staticMembers)
                {
                    yield return (id, ToMember(id, member));
                }
            }

            return new NativeClass(name, Primitive.Object, Get, Set, List, instanceMembers);
        }

        public static object? ToCSharpOperand(Operand o)
        {
            // TODO maybe make this an abstract method for operands
            return o switch
            {
                Value<int> i => i.Underlying,
                Value<float> f => f.Underlying,
                Value<bool> b => b.Underlying,
                Operands.String s => s.Underlying,
                Value.NullClass _ => null,
                Operands.Array a => a.Values().Select(val => ToCSharpOperand(val)),
                _ => throw new NotImplementedException()
            };
        }

        public static Types.Type Convert(System.Type input)
        {
            if (input.IsArray) return new ArrayType(Convert(input.GetElementType() ?? typeof(object)));
            if (input.IsGenericType && input.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return new ArrayType(Convert(input.GetGenericArguments()[0]));
            if (Conversions.OutletType.ContainsKey(input))
                return Conversions.OutletType[input];
            return Primitive.Object;
        }

        public static FunctionType ToOutletMethodType(MethodInfo method) =>
            new FunctionType(method.GetParameters()
                .Select(param => (Convert(param.ParameterType), param.Name!))
                .ToArray(), Convert(method.ReturnType));

        public static NativeFunction Convert(string name, MethodInfo method) =>
            new NativeFunction(name, ToOutletMethodType(method), method);

        public static FunctionType ToOutletConstructorType(ConstructorInfo constructor) =>
            new FunctionType(constructor.GetParameters()
                .Select(param => (Convert(param.ParameterType), param.Name!))
                .ToArray(), Convert(constructor.DeclaringType!));

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
            foreach (var type in classes)
            {
                ForeignClass fc = type.GetCustomAttribute(typeof(ForeignClass))
                        is ForeignClass rc ? rc : throw new UnexpectedException("unexpected error");
                string className = !string.IsNullOrEmpty(fc.Name) ? fc.Name : type.Name;

                var fields = GetFields(type);
                var methods = GetMethods(type);
                var constructors = GetConstructors(type);

                var staticMembers = new List<(string id, MemberInfo member)>();
                var staticTypes = new CheckStackFrame(null);
                var instanceMembers = new List<(string id, MemberInfo member)>();
                var instanceTypes = new CheckStackFrame(null);

                // Checktime
                // Add Create and define ProtoClass first, allowing members to reference the type they are declared in
                ProtoClass proto = new ProtoClass(className, Primitive.Object, staticTypes, instanceTypes);
                MetaType checkTimeType = new MetaType(proto);
                Conversions.OutletType.Add(type, proto);
                CheckStackFrame.Global.Assign(className.ToVariable(), checkTimeType);

                foreach (FieldInfo field in fields)
                {
                    ForeignField ff = field.GetCustomAttribute(typeof(ForeignField))
                        is ForeignField rf ? rf : throw new UnexpectedException("unexpected error");
                    string ffName = ff.Name ?? field.Name;
                    if (field.IsStatic)
                    {
                        staticMembers.Add((ffName, field));
                        staticTypes.Assign(ffName.ToVariable(), Convert(field.FieldType));
                    }
                    else
                    {
                        instanceMembers.Add((ffName, field));
                        instanceTypes.Assign(ffName.ToVariable(), Convert(field.FieldType));
                    }
                }

                foreach (MethodInfo method in methods)
                {
                    ForeignFunction fm = method.GetCustomAttribute(typeof(ForeignFunction))
                        is ForeignFunction rm ? rm : throw new UnexpectedException("unexpected error");
                    string fmName = fm.Name ?? method.Name;
                    if (method.IsStatic)
                    {
                        staticMembers.Add((fmName, method));
                        staticTypes.Assign(fmName.ToVariable(), ToOutletMethodType(method));
                    }
                    else
                    {
                        instanceMembers.Add((fmName, method));
                        instanceTypes.Assign(fmName.ToVariable(), ToOutletMethodType(method));
                    }
                }

                foreach (ConstructorInfo constructor in constructors)
                {
                    staticMembers.Add(("", constructor));
                    staticTypes.Assign("".ToVariable(), ToOutletConstructorType(constructor));
                }


                // Runtime
                NativeClass c = ToOutletClass(className, staticMembers.ToArray(), instanceMembers.ToArray());
                ForeignFunctions.NativeTypes.Add(className, c);
                Conversions.OutletType[type] = c;
            };
        }
    }
}
