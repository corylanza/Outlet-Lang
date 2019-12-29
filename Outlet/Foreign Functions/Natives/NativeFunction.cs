using Outlet.Operands;
using Outlet.Types;
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
            NativeInitializer.ToOutletOperand(Underlying.Invoke(null, args.Select(arg => NativeInitializer.ToCSharpOperand(arg)).ToArray()));
    }

    public class NativeConstructor : Function
    {
        private readonly ConstructorInfo Underlying;

        public NativeConstructor(string name, FunctionType type, ConstructorInfo func) : base(name, type)
        {
            Underlying = func;
        }

        public override Operand Call(params Operand[] args) =>
            NativeInitializer.ToOutletInstance(RuntimeType.ReturnType as Class, Underlying.Invoke(args.Select(arg => NativeInitializer.ToCSharpOperand(arg)).ToArray()));
    }
}
