using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;

namespace Outlet.FFI
{
    public static class FFIConfig
    {

        private static readonly Dictionary<System.Type, Operands.Type> Conversions = new Dictionary<System.Type, Operands.Type>() {
            {typeof(object), Primitive.Object },
            {typeof(string), Primitive.String },
            {typeof(int), Primitive.Int },
            {typeof(bool), Primitive.Bool },
            {typeof(float), Primitive.Float },
            {typeof(void), Primitive.Void }
        };

        public static Operand FromNative(object o)
        {

            return o is null ? null : Conversions.GetValueOrDefault(o.GetType()) switch
            {
                Primitive p when p == Primitive.Int => new Constant((int) o),
                Primitive p when p == Primitive.Float => new Constant((float) o),
                Primitive p when p == Primitive.String => new Constant((string) o),
                Primitive p when p == Primitive.Bool => new Constant((bool) o),
                null => null,
                _ => throw new Exception("Cannot map type")
            };
        }

        public static object ToNative(Operand o)
        {
            // TODO maybe make this an abstract method for operands
            return o switch
            {
                Constant c => c.Value,
                Operands.Array a => a.Values().Select(val => ToNative(val)),
                _ => throw new NotImplementedException()
            };
        }

        public static Operands.Type Convert(System.Type input) => Conversions[input];

        public static NativeFunction Convert(string name, MethodInfo method)
        {
            var type = new FunctionType(method.GetParameters()
                .Select(param => (Convert(param.ParameterType), param.Name))
                .ToArray(), Convert(method.ReturnType));
            return new NativeFunction(name, type, method);
        }

        public static Operand Convert(FieldInfo field)
        {
            return FromNative(field.GetValue(null));
        }

        private static NativeFunction GenerateConstructor(NativeClass nc)
        {
            Instance Hidden()
            {
                return null;
            }
            //return new Native(new FunctionType(nc), Hidden.);
            return null;
        }

        private static IEnumerable<System.Type> GetForeignClasses() => 
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && x.GetCustomAttributes(typeof(ForeignClass)).FirstOrDefault() != null);

        private static IEnumerable<MethodInfo> GetMethods(System.Type type) => 
            type.GetMethods().Where(x => x.GetCustomAttributes(typeof(ForeignFunction), false).FirstOrDefault() != null);

        private static IEnumerable<FieldInfo> GetFields(System.Type type) =>
            type.GetFields().Where(x => x.GetCustomAttributes(typeof(ForeignField), false).FirstOrDefault() != null);

        private static IEnumerable<ConstructorInfo> GetConstructors(System.Type type) =>
            type.GetConstructors().Where(x => x.GetCustomAttributes(typeof(ForeignConstructor)).FirstOrDefault() != null);

        public static void Register()
        {
            var classes = GetForeignClasses();
            foreach(var type in classes)
            {
                // Register each class in conversions first
                ForeignClass fc = (ForeignClass)type.GetCustomAttribute(typeof(ForeignClass));
                
                string className = !string.IsNullOrEmpty(fc.Name) ? fc.Name : type.Name;

                //Conversions.Add(type, new NativeClass(className, null, GetMethods(type), GetFields(type), GetConstructors(type)));
                Conversions.Add(type, new NativeClass(className, null, GetMethods(type).Union<MemberInfo>(GetFields(type)).Union(GetConstructors(type))));
            }
            foreach(NativeClass nc in Conversions.Values.Where(type => type is NativeClass))
            {
                nc.RegisterMembers();

                ForeignFunctions.NativeTypes.Add(nc.Name, nc);
            }
        }
    }
}
