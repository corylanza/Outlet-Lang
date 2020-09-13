using Outlet.Operands;
using Outlet.StandardLib;
using Outlet.Types;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Outlet.FFI.Natives
{
    public class NativeFunction : Function
    {
        private readonly MethodInfo Underlying;
        private readonly SystemInterface System;

        public NativeFunction(string name, FunctionType type, MethodInfo func, SystemInterface system) : base(name, type)
        {
            Underlying = func;
            System = system;
        }

        public override Operand Call(Operand? caller, params Operand[] _args) {
            // null if the method is static
            object? instance = caller is NativeInstance ni ? ni.Underlying : null;
            var enumerator = _args.Select(arg => NativeInitializer.ToCSharpOperand(arg)).GetEnumerator();
            // If there is a parameter sys of type SystemInterface, dependency inject System
            object?[] args = Underlying.GetParameters().Select(param =>
            {
                if (param.ParameterType == typeof(SystemInterface) && param.Name == "sys")
                {
                    return System;
                } else
                {
                    enumerator.MoveNext();
                    var arg = enumerator.Current;
                    return arg;
                } 
            }).ToArray();
            var res = Underlying.Invoke(instance, args);
            return NativeInitializer.ToOutletOperand(res, System);
        }
    }

    public class NativeConstructor : Function
    {
        private readonly ConstructorInfo Underlying;
        private readonly SystemInterface System;

        public NativeConstructor(string name, FunctionType type, ConstructorInfo func, SystemInterface system) : base(name, type)
        {
            Underlying = func;
            System = system;
        }

        public override Operand Call(Operand? caller, params Operand[] args) =>
            NativeInitializer.ToOutletInstance(RuntimeType.ReturnType is NativeClass nc ? nc : throw new UnexpectedException("Expected Native Class"),
            Underlying.Invoke(args.Select(arg => NativeInitializer.ToCSharpOperand(arg)).ToArray()), System);
    }
}
