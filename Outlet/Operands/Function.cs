﻿using Outlet.Types;
using System.Collections.Generic;

namespace Outlet.Operands {

    public abstract class Function : Operand<FunctionType>, ICallable
    {
        public readonly string Name;
        public override FunctionType RuntimeType { get; }

        protected Function(string name, FunctionType type) => (Name, RuntimeType) = (name, type);

        public abstract Operand Call(Operand? caller, List<Type> typeArgs, params Operand[] args);

        public override bool Equals(Operand b) => ReferenceEquals(this, b);

        public override string ToString()
        {
            return Name + RuntimeType.ToString();
        }

        public bool Valid(out uint level, params Type[] inputs) => RuntimeType.Valid(out level, inputs);
    }

	public class UserDefinedFunction : Function 
    {

		private readonly CallFunc Hidden;

        public UserDefinedFunction(string id, FunctionType type, CallFunc act) : base(id, type) 
        {
			Hidden = act;
		}

		public override Operand Call(Operand? caller, List<Type> typeArgs, params Operand[] args) => Hidden(args);
	}


    public delegate Operand CallFunc(params Operand[] args);
    public delegate Operand BinaryOperation(Operand left, Operand right);
    public delegate Operand UnaryOperation(Operand expr);
}
