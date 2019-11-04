using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet {
    public class UnOp : IOverloadable {

		public readonly Type Input, Result;
		private readonly Func<Operand, Operand> Native;
		private readonly UserDefinedFunction UserDefined;

        public UnOp(Type input, Type result, Func<Operand, Operand> f) {
			Input = input;
			Result = result;
			Native = f;
        }

		public UnOp(Type input, Type result, UserDefinedFunction f) {
			Input = input;
			Result = result;
			UserDefined = f;
		}

		public Operand Perform(Operand o) {
			return Native(o);
		}

		public bool Valid(out int level, params Type[] inputs) {
            if (inputs.Length != 1) 
            {
                level = -1;
                return false;
            }
            return inputs[0].Is(Input, out level);
		}
	}

	public class BinOp : IOverloadable {

		public readonly Type Left, Right, Result;
		private readonly Func<Operand, Operand, Operand> Native;

		public BinOp(Type l, Type r, Type res, Func<Operand, Operand, Operand> f) {
			(Left, Right, Result, Native) = (l, r, res, f);
		}

		public Operand Perform(Operand l, Operand r) {
			return Native(l, r);
		}

        public bool Valid(out int level, params Type[] inputs)
        {
            if (inputs.Length == 2 && inputs[0].Is(Left, out int l) && inputs[1].Is(Right, out int r))
            {
                level = l + r;
                return true;
            }
            level = -1;
            return false;
        }
	}
}
