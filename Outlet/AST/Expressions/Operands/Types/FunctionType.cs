using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class FunctionType : Type {

		private readonly Type Args, Return;

		public FunctionType(Type args, Type returntype) : base(Primitive.FuncType, null) {
			Args = args;
			Return = returntype;
		}

		public override Operand Dereference(string field) {
			throw new NotImplementedException();
		}

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
		}

		public override string ToString() => Args.ToString() + " => " + Return.ToString();
	}
}
