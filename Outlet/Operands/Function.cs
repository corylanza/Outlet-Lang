﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {

	public class Function : Operand, ICallable {

		private readonly string Name;
		private readonly CallFunc Hidden;

		protected Function() { }

		public Function(string id, FunctionType type, CallFunc act) {
			Name = id;
			Type = type;
			Hidden = act;
		}

		public virtual Operand Call(params Operand[] args) => Hidden(args);

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override string ToString() {
			return Name+Type.ToString();
		}

	}

	public class Native : Function {

		private readonly CallFunc Underlying;

		public Native(FunctionType type, CallFunc func) {
			Underlying = func;
			Type = type;
		}

		public override Operand Call(params Operand[] args) => Underlying(args);
	}
	
}