using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Expressions {
	public class OTuple : Operand {

		public OTuple(params Expression[] vals) {
			Value = vals;
		}

		public override Operand Eval() {
			for(int i = 0; i < Value.Length; i++) {
				Value[i] = Value[i].Eval();
			}
			return this;
		}

		public override string ToString() {
			string s = "(";
			foreach(Expression e in Value) {
				s += e.ToString() + ", ";
			}
			return s.Substring(0, s.Length-2) + ")";
		}
	}
}
