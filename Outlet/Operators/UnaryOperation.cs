using Outlet.Operands;
using Outlet.Types;
using System;
using Type = Outlet.Types.Type;

namespace Outlet.Operators
{
    public abstract class UnOp : IOverloadable
    {
        protected Type Input, Output;

        protected UnOp(Type input, Type output) => (Input, Output) = (input, output);

        public abstract Operand Perform(Operand input);

        public bool Valid(out uint level, params Type[] inputs)
        {
            if (inputs.Length != 1)
            {
                level = 0;
                return false;
            }
            return inputs[0].Is(Input, out level);
        }

        public Type GetResultType() => Output;
    }

    public class UnOp<I, O> : UnOp where I : Operand where O : Operand
    {
        private readonly Func<I, O> Underlying;

        public UnOp(Func<I, O> func) : base(Conversions.GetRuntimeType<I>(), Conversions.GetRuntimeType<O>())
        {
            Underlying = func;
        }

        public override Operand Perform(Operand input) =>
            input is I arg ? Underlying(arg) : throw new OutletException("invalid operation for type SHOULD NOT PRINT");
    }

    public class UserDefinedUnaryOperation : UnOp
    {
        private readonly UnaryOperation Underlying;

        public UserDefinedUnaryOperation(UnaryOperation underlying, Type input, Type output) : base(input, output)
        {
            Underlying = underlying;
        }

        public override Operand Perform(Operand input) => Underlying(input);
    }
}
