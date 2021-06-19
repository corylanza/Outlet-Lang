using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Compiling.Instructions
{
    public class ConstInt : Instruction
    {
        public int Value { get; set; }

        public ConstInt(int value) => Value = value;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(ConstInt)} {Value}";
    }

    public class ConstFloat : Instruction
    {
        public float Value { get; set; }

        public ConstFloat(float value) => Value = value;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(ConstFloat)} {Value}";
    }

    public class ConstBool : Instruction
    {
        public bool Value { get; set; }

        public ConstBool(bool value) => Value = value;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(ConstBool)} {Value}";
    }

    public class ConstString : Instruction
    {
        public string Value { get; set; }

        public ConstString(string value) => Value = value;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);


        public override string ToString() => $"{nameof(ConstString)} {Value}";
    }

    public class NegateInt : Instruction
    {
        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(NegateInt)}";
    }

    public class BinaryAdd : Instruction
    {
        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(BinaryAdd)}";
    }

    public class BinarySub : Instruction
    {
        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(BinarySub)}";
    }

    public class LocalStore : Instruction
    {

        public uint LocalId { get; set; }

        public LocalStore(uint localId) => LocalId = localId;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(LocalStore)} {LocalId}";
    }

    public class LocalGet : Instruction
    {

        public uint LocalId { get; set; }

        public LocalGet(uint localId) => LocalId = localId;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(LocalGet)} {LocalId}";
    }

    public class JumpRelative : Instruction
    {

        public int JumpInterval { get; set; }

        public JumpRelative(int jumpInterval) => JumpInterval = jumpInterval;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(JumpRelative)} {JumpInterval}";
    }

    public class JumpFalseRelative : Instruction
    {

        public int JumpInterval { get; set; }

        public JumpFalseRelative(int jumpInterval) => JumpInterval = jumpInterval;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{nameof(JumpFalseRelative)} {JumpInterval}";
    }

    public interface IInstructionVisitor<T>
    {
        T Visit(ConstInt c);
        T Visit(ConstFloat c);
        T Visit(ConstBool c);
        T Visit(ConstString c);
        T Visit(NegateInt n);
        T Visit(BinaryAdd b);
        T Visit(BinarySub b);
        T Visit(LocalStore l);
        T Visit(LocalGet l);
        T Visit(JumpRelative j);
        T Visit(JumpFalseRelative j);
    }
}
