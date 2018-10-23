using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class OList : Operand, ICallable, IDereferenceable, ICollection {
		public OList(params Operand[] vals) {
			Type = Type.List;
			Value = vals;
		}
		public override Operand Eval(Scope scope) => this;

		public Operand Dereference(Identifier field) {
			if (field.Name == "length") return new Literal(Value.Length);
			throw new OutletException("field "+field.Name+" not defined");
		}

		public Operand Call(params Operand[] args) => Value[args[0].Value];

		public Operand[] Values() => Value;

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
