using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Type = Outlet.AST.Type;

namespace Outlet {
    public class UnaryOperation {

		public readonly Type Input, Result;
		private readonly Func<Operand, Operand> Native;
		private readonly Function UserDefined;

        public UnaryOperation(Type input, Type result, Func<Operand, Operand> f) {
			Input = input;
			Result = result;
			Native = f;
        }

		public UnaryOperation(Type input, Type result, Function f) {
			Input = input;
			Result = result;
			UserDefined = f;
		}

		public Operand Perform(Operand o) => Native(o);
    }
}
