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
                NativeClass nc => ToOutletInstance(nc, o),
                null => Constant.Null(),
                _ => throw new Exception("Cannot map type")
            };
        }

        public static Operand ToOutletInstance(NativeClass nc, object o)
        {
            static Operand ToMember(string id, MemberInfo member, object o) => member switch
            {
                MethodInfo method => Convert(id, method),
                FieldInfo field => ToOutletOperand(o.GetType().GetField(field.Name).GetValue(o)),
                _ => throw new NotSupportedException()
            };

            Operand Get(IBindable id) {
                var member = nc.InstanceMembers[id.LocalId];
                return ToMember(member.id, member.member, o);
            }

            void Set(IBindable id, Operand val)
            {
                if(nc.InstanceMembers[id.LocalId].member is FieldInfo field) field.DeclaringType.GetField(field.Name).SetValue(o, ToCSharpOperand(val));
            }

            IEnumerable<(string id, Operand val)> List()
            {
                foreach ((string id, MemberInfo member) in nc.InstanceMembers)
                {
                    yield return (id, ToMember(id, member, o));
                }
            }

            return new NativeInstance(nc, Get, Set, List);
        }

        public static NativeClass ToOutletClass(string name, (string id, MemberInfo member)[] staticMembers, (string, MemberInfo)[] instanceMembers)
        {
            static Operand ToMember(string id, MemberInfo member) => member switch
            {
                MethodInfo method => Convert(id, method),
                FieldInfo field => ToOutletOperand(field.DeclaringType.GetField(field.Name).GetValue(null)),
                ConstructorInfo constructor => Convert(constructor),
                _ => throw new NotSupportedException()
            };

            Operand Get(IBindable id) {
                var member = staticMembers[id.LocalId];
                return ToMember(member.id, member.member);
            };

            void Set(IBindable id, Operand val)
            {
                if(staticMembers[id.LocalId].member is FieldInfo field) field.DeclaringType.GetField(field.Name).SetValue(null, ToCSharpOperand(val));
            }
            IEnumerable<(string id, Operand val)> List()
            {
                foreach((string id, MemberInfo member) in staticMembers)
                {
                    yield return (id, ToMember(id, member));
                }
            }

            return new NativeClass(name, Primitive.Object, Get, Set, List, instanceMembers);
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

                var fields = GetFields(type);
                var methods = GetMethods(type);
                var constructors = GetConstructors(type);

                var staticMembers = new List<(string id, MemberInfo member)>();
                var staticTypes = new SymbolTable(null, false);
                var instanceMembers = new List<(string id, MemberInfo member)>();
                var instanceTypes = new SymbolTable(null, false);

                // Checktime
                // Add Create and define ProtoClass first, allowing members to reference the type they are declared in
                ProtoClass proto = new ProtoClass(className, Primitive.Object, staticTypes, instanceTypes);
                TypeObject CheckTime = new TypeObject(proto);
                Conversions.OutletType.Add(type, proto);
                SymbolTable.Global.Define(CheckTime, className.ToVariable(), () => SymbolTable.Global.List().Count());

                int staticCount = 0;
                int getNextStatic() => staticCount++;
                int instanceCount = 0;
                int getNextInstance() => instanceCount++;

                foreach (FieldInfo field in fields)
                {
                    ForeignField ff = ((ForeignField)field.GetCustomAttribute(typeof(ForeignField)));
                    string ffName = ff.Name ?? field.Name;
                    if (field.IsStatic)
                    {
                        staticMembers.Add((ffName, field));
                        staticTypes.Define(Convert(field.FieldType), ffName.ToVariable(), getNextStatic);
                    }
                    else
                    {
                        instanceMembers.Add((ffName, field));
                        instanceTypes.Define(Convert(field.FieldType), ffName.ToVariable(), getNextInstance);
                    }
                }

                foreach (MethodInfo method in methods)
                {
                    ForeignFunction fm = ((ForeignFunction)method.GetCustomAttribute(typeof(ForeignFunction)));
                    string fmName = fm.Name ?? method.Name;
                    if (method.IsStatic)
                    {
                        staticMembers.Add((fmName, method));
                        staticTypes.Define(ToOutletMethodType(method), fmName.ToVariable(), getNextStatic);
                    }
                    else
                    {
                        instanceMembers.Add((fmName, method));
                        instanceTypes.Define(ToOutletMethodType(method), fmName.ToVariable(), getNextInstance);
                    }
                }

                foreach (ConstructorInfo constructor in constructors)
                {
                    staticMembers.Add(("", constructor));
                    staticTypes.Define(ToOutletConstructorType(constructor), "".ToVariable(), getNextStatic);
                }


                // Runtime
                NativeClass c = ToOutletClass(className, staticMembers.ToArray(), instanceMembers.ToArray());
                ForeignFunctions.NativeTypes.Add(className, c);
                Conversions.OutletType[type] = c;
            };
        }
    }
}
