using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class FunctionType : Type, IOverloadable {

		public readonly (Type type, string id)[] Args;
		public readonly Type ReturnType;

		public FunctionType((Type, string)[] args, Type returntype) : base(Primitive.Object, null) {
			Args = args;
			ReturnType = returntype;
		}

		public override bool Is(Type t) {
			if(t == Primitive.Object) return true;
			return false;// t.Is(Primitive.FuncType);
		}

		public override bool Is(Type t, out int level) {
			throw new NotImplementedException();
		}

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
		}

		public override string ToString() => "("+Args.Select(arg => arg.type).ToList().ToListString()+")" + " => " + ReturnType.ToString();

		public bool Valid(params Type[] inputs) {
			throw new NotImplementedException();
		}

		public int Level(params Type[] inputs) {
			throw new NotImplementedException();
		}
	}
}
