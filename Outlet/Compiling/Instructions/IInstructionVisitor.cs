﻿using System;
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
    }

    public class ConstFloat : Instruction
    {
        public float Value { get; set; }

        public ConstFloat(float value) => Value = value;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);
    }

    public class ConstBool : Instruction
    {
        public bool Value { get; set; }

        public ConstBool(bool value) => Value = value;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);
    }

    public class ConstString : Instruction
    {
        public string Value { get; set; }

        public ConstString(string value) => Value = value;

        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);
    }

    public class NegateInt : Instruction
    {
        public override T Accept<T>(IInstructionVisitor<T> visitor) => visitor.Visit(this);
    }

    public interface IInstructionVisitor<T>
    {
        T Visit(ConstInt c);
        T Visit(ConstFloat c);
        T Visit(ConstBool c);
        T Visit(ConstString c);
        T Visit(NegateInt n);
    }
}