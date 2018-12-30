using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class ArrayType : Type {

		public Type ElementType;

		public ArrayType(Type elem) : base (null, null) {
			ElementType = elem;
		}

		public override bool Equals(Operand b) => b is ArrayType at && ElementType.Equals(at.ElementType);

		public override bool Is(Type t) => t is ArrayType at && ElementType.Is(at.ElementType);

		public override bool Is(Type t, out int level) {
			throw new NotImplementedException();
		}

		public override string ToString() => ElementType.ToString() + "[]";
	}
}
