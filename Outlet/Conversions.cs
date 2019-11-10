using System;
using System.Collections.Generic;
using System.Text;
using Outlet.Operands;

namespace Outlet
{
    public class Conversions
    {
        private Dictionary<System.Type, Operands.Type> Compiled = new Dictionary<System.Type, Operands.Type>
        {
            {typeof(Constant<int>), Primitive.Int},
            {typeof(Constant<int>), Primitive.Int},
            {typeof(Constant<int>), Primitive.Int},
            {typeof(Constant<int>), Primitive.Int},
            {typeof(Constant<int>), Primitive.Int},
            {typeof(Operands.Type), Primitive.Int},
        };



        public static Operands.Type GetRuntimeType<T>() where T : Operand
        {
            if (typeof(T) == typeof(Constant<int>)) return Primitive.Int;
            if (typeof(T) == typeof(Constant<bool>)) return Primitive.Bool;
            if (typeof(T) == typeof(Constant<float>)) return Primitive.Float;
            if (typeof(T) == typeof(Constant<string>)) return Primitive.String;
            if (typeof(T) == typeof(Constant<object>)) return Primitive.Object;
            if (typeof(T) == typeof(Operand)) return Primitive.Object;
            if (typeof(T) == typeof(Operands.Type) || typeof(T).IsSubclassOf(typeof(Operands.Type))) return Primitive.MetaType;
            throw new Exception(typeof(T).FullName + " is not a compile tiime constant type");
        }
    }
}
