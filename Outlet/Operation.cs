using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet
{
    public abstract class UnOp : IOverloadable
    {
        protected Type Input, Output;

        public UnOp(Type input, Type output) => (Input, Output) = (input, output);

        public abstract Operand Perform(Operand input);

        public abstract bool Valid(out int level, params Type[] inputs);
        public abstract Type GetResultType();
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

        public override bool Valid(out int level, params Type[] inputs)
        {
            if (inputs.Length != 1)
            {
                level = -1;
                return false;
            }
            return inputs[0].Is(Input, out level);
        }

        public override Type GetResultType() => Output;
    }

    public abstract class BinOp : IOverloadable
    {

        protected Type LeftInput, RightInput, Output;

        public BinOp(Type left, Type right, Type output) => (LeftInput, RightInput, Output) = (left, right, output);

        public abstract Operand Perform(Operand l, Operand r);
        public abstract bool Valid(out int level, params Type[] inputs);
        public abstract Type GetResultType();
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

        public override bool Valid(out int level, params Type[] inputs)
        {
            if (inputs.Length == 2 && inputs[0].Is(LeftInput, out int l) && inputs[1].Is(RightInput, out int r))
            {
                level = l + r;
                return true;
            }
            level = -1;
            return false;
        }

        public override Type GetResultType() => Output;
    }
}

