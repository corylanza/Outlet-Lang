using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class FunctionType : Type {

		public readonly (Type type, string id)[] Args;
		public readonly Type ReturnType;

		public FunctionType((Type, string)[] args, Type returntype) : base(Primitive.FuncType, null) {
			Args = args;
			ReturnType = returntype;
		}

		public override bool Is(Type t) {
			return t.Is(Primitive.FuncType);
		}

		public override bool Is(Type t, out int level) {
			throw new NotImplementedException();
		}

		public override Operand Dereference(string field) {
			throw new NotImplementedException();
		}

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
		}

		public override string ToString() => "("+Args.Select(arg => arg.type).ToList().ToListString()+")" + " => " + ReturnType.ToString();
	}
}
