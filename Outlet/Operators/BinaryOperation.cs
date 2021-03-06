﻿using Outlet.Operands;
using Outlet.Types;
using System;
using Type = Outlet.Types.Type;

namespace Outlet.Operators
{
    public abstract class BinOp : IOverloadable
    {
        protected Type LeftInput, RightInput, Output;

        protected BinOp(Type left, Type right, Type output) => (LeftInput, RightInput, Output) = (left, right, output);

        public abstract Operand Perform(Operand l, Operand r);
        public Type GetResultType() => Output;

        public bool Valid(out uint level, params Type[] inputs)
        {
            if (inputs.Length == 2 && inputs[0].Is(LeftInput, out uint l) && inputs[1].Is(RightInput, out uint r))
            {
                level = l + r;
                return true;
            }
            level = 0;
            return false;
        }
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
    }

    public class UserDefinedBinaryOperation : BinOp
    {
        private readonly BinaryOperation Underlying;

        public UserDefinedBinaryOperation(BinaryOperation underlying, Type left, Type right, Type output) : base(left, right, output)
        {
            Underlying = underlying;
        }

        public override Operand Perform(Operand l, Operand r) => Underlying(l, r);
    }
}
