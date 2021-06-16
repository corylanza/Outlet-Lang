using Outlet.Compiling.Instructions;
using Outlet.Operands;
using Outlet.Types;
using System;
using System.Collections.Generic;
using Type = Outlet.Types.Type;

namespace Outlet.Operators
{
    public abstract class UnaryOperation : IOverloadable
    {
        protected Type Input, Output;
        private readonly Func<Instruction> InstructionGen;

        protected UnaryOperation(Type input, Type output) : this(input, output, null) { }

        protected UnaryOperation(Type input, Type output, Func<Instruction>? bytecode)
        {
            (Input, Output) = (input, output);
            InstructionGen = bytecode ?? NotImplementedInstruction;
        }

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

        private Instruction NotImplementedInstruction() => throw new NotImplementedException();

        public IEnumerable<Instruction> GenerateByteCode()
        {
            yield return InstructionGen();
        }
    }

    public class UnOp<I, O> : UnaryOperation where I : Operand where O : Operand
    {
        private readonly Func<I, O> Underlying;

        public UnOp(Func<I, O> func, Func<Instruction>? bytecode = null) : base(Conversions.GetRuntimeType<I>(), Conversions.GetRuntimeType<O>(), bytecode)
        {
            Underlying = func;
        }

        public override Operand Perform(Operand input) =>
            input is I arg ? Underlying(arg) : throw new OutletException("invalid operation for type SHOULD NOT PRINT");
    }


    public delegate Operand UnderlyingUnaryOperation(Operand expr);

    public class UserDefinedUnaryOperation : UnaryOperation
    {
        private readonly UnderlyingUnaryOperation Underlying;

        public UserDefinedUnaryOperation(UnderlyingUnaryOperation underlying, Type input, Type output) : base(input, output)
        {
            Underlying = underlying;
        }

        public override Operand Perform(Operand input) => Underlying(input);
    }
}
