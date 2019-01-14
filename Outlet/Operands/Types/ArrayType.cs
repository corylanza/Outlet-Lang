﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class ArrayType : Type {

		public Type ElementType;

		public ArrayType(Type elem) {
			ElementType = elem;
		}

		public override bool Equals(Operand b) => b is ArrayType at && ElementType.Equals(at.ElementType);

		public override bool Is(Type t) => t == Primitive.Object || t is ArrayType at && ElementType.Is(at.ElementType);

		public override bool Is(Type t, out int level) {
			throw new NotImplementedException();
		}

		public override string ToString() => ElementType.ToString() + "[]";
	}
}