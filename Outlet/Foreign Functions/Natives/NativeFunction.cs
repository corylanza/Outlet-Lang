using Outlet.Operands;
using System.Linq;
using System.Reflection;

namespace Outlet.FFI.Natives
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

    public class NativeConstructor : Function
    {
        private readonly ConstructorInfo Underlying;

        public NativeConstructor(string name, FunctionType type, ConstructorInfo func) : base(name, type)
        {
            Underlying = func;
        }

        public override Operand Call(params Operand[] args) =>
            FFIConfig.FromNativeInstance(Type.ReturnType as NativeClass, Underlying.Invoke(args.Select(arg => FFIConfig.ToNative(arg)).ToArray()));
    }
}
