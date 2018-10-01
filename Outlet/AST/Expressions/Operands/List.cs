using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class OList : Operand {
		public OList(params Expression[] vals) {
			Value = vals;
		}
		public override Operand Eval(Scope block) {
			for (int i = 0; i < Value.Length; i++) {
				Value[i] = Value[i].Eval(block);
			}
			return this;
		}

		public override bool Equals(Operand b) {
			if (b is OList oth) {
				if (Value.Length != b.Value.Length) return false;
				for (int i = 0; i < Value.Length; i++) {
					if (!Value[i].Equals(oth.Value[i])) return false;
				}
				return true;
			}
			return false;
		}

		public override string ToString() {
			if (Value.Length == 0) return "List()";
			 string s = "List(";
			foreach (Expression e in Value) {
				s += e.ToString() + ", ";
			}
			return s.Substring(0, s.Length - 2) + ")";
		}
	}
}
