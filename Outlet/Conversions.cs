using System;
using System.Collections.Generic;
using Outlet.Operands;
using Outlet.Types;

namespace Outlet
{
    public class Conversions
    {
        public static ITyped GetRuntimeType<T>() where T : Operand
        {
            if (typeof(T) == typeof(Constant<int>)) return Primitive.Int;
            if (typeof(T) == typeof(Constant<bool>)) return Primitive.Bool;
            if (typeof(T) == typeof(Constant<float>)) return Primitive.Float;
            if (typeof(T) == typeof(Constant<string>)) return Primitive.String;
            if (typeof(T) == typeof(Constant<object>)) return Primitive.Object;
            if (typeof(T) == typeof(Operand)) return Primitive.Object;
            if (typeof(T) == typeof(TypeObject)) return Primitive.MetaType;
            throw new Exception(typeof(T).FullName + " is not a compile tiime constant type");
        }
    }
}
