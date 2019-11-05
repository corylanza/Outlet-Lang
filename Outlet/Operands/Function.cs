using Outlet;
using System;
using System.Collections.Generic;

namespace Outlet.Operands {

    public abstract class Function : Operand<FunctionType>, ICallable, IOverloadable
    {
        public readonly string Name;
        public Function(string name, FunctionType type) => (Name, Type) = (name, type);

        public abstract Operand Call(params Operand[] args);

        public override bool Equals(Operand b) => ReferenceEquals(this, b);

        public override string ToString()
        {
            return Name + Type.ToString();
        }

        public bool Valid(out int level, params Type[] inputs)
        {
            return (Type as FunctionType).Valid(out level, inputs);
        }
    }

	public class UserDefinedFunction : Function 
    {

		private readonly CallFunc Hidden;
        public delegate Operand CallFunc(params Operand[] args);

        public UserDefinedFunction(string id, FunctionType type, CallFunc act) : base(id, type) 
        {
			Hidden = act;
		}

		public override Operand Call(params Operand[] args) => Hidden(args);
	}
}
