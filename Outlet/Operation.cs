using System;
using Outlet.Operands;
using Outlet.Types;

namespace Outlet
{
    public abstract class UnOp : IOverloadable
    {
        protected ITyped Input, Output;

        public UnOp(ITyped input, ITyped output) => (Input, Output) = (input, output);

        public abstract Operand Perform(Operand input);

        public abstract bool Valid(out int level, params ITyped[] inputs);
        public abstract ITyped GetResultType();
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

        public override bool Valid(out int level, params ITyped[] inputs)
        {
            if (inputs.Length != 1)
            {
                level = -1;
                return false;
            }
            return inputs[0].Is(Input, out level);
        }

        public override ITyped GetResultType() => Output;
    }

    public abstract class BinOp : IOverloadable
    {

        protected ITyped LeftInput, RightInput, Output;

        public BinOp(ITyped left, ITyped right, ITyped output) => (LeftInput, RightInput, Output) = (left, right, output);

        public abstract Operand Perform(Operand l, Operand r);
        public abstract bool Valid(out int level, params ITyped[] inputs);
        public abstract ITyped GetResultType();
    }

    public class BinOp<L, R, O> : BinOp where L : Operand where R : Operand where O : Operand
    {
        private readonly Func<L, R, O> Underlying;

        public BinOp(Func<L, R, O> func) : base(
            Conversions.GetRuntimeType<L>(), 
            Conversions.GetRuntimeType<R>(), 
            Conversions.GetRuntimeType<O>())
        {
            Underlying = func;
        }
        public override Operand Perform(Operand l, Operand r) => 
            l is L lArg &&
            r is R rArg ? Underlying(lArg, rArg) : throw new OutletException("Cannot perform operation as types do not match compile time types. SHOULD NOT PRINT");

        public override bool Valid(out int level, params ITyped[] inputs)
        {
            if (inputs.Length == 2 && inputs[0].Is(LeftInput, out int l) && inputs[1].Is(RightInput, out int r))
            {
                level = l + r;
                return true;
            }
            level = -1;
            return false;
        }

        public override ITyped GetResultType() => Output;
    }
}

