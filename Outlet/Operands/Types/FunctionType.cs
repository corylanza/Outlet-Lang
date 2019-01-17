using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class FunctionType : Type {

		public readonly (Type type, string id)[] Args;
		public readonly Type ReturnType;

		public FunctionType((Type, string)[] args, Type returntype) {
			Args = args;
			ReturnType = returntype;
		}

		public override bool Is(Type t) {
			if(t == Primitive.Object) return true;
			if(t is FunctionType ft && Args.Length == ft.Args.Length) {
				for(int i = 0; i < Args.Length; i++) {
					if(!Args[i].type.Is(ft.Args[i].type)) return false;
				}
				return ReturnType.Is(ft.ReturnType);
			} return false;
		}

		public override bool Is(Type t, out int level) {
			throw new NotImplementedException();
		}

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
		}

		public override string ToString() => "("+Args.Select(arg => arg.type).ToList().ToListString()+")" + " => " + ReturnType.ToString();

	}
}
