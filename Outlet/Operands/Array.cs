using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class Array : Operand {
		public Array(params Operand[] vals) {
			if(vals.Length == 0) Type = new ArrayType(Primitive.Object);
			else {
				Type ancestor = vals[0].Type;
				foreach(Operand cur in vals) {
					ancestor = Type.CommonAncestor(ancestor, cur.Type);
				}
				Type = new ArrayType(ancestor);
			}
			Value = vals;
		}
		/*
		public Operand Dereference(string field) {
			if (field == "length") return new Constant(Value.Length);
			throw new OutletException("field "+field+" not defined");
		}*/

		//public Operand Call(params Operand[] args) => Value[args[0].Value];

		public Operand[] Values() => Value;

		public override bool Equals(Operand b) {
			if (b is Array oth) {
				if(Value.Length != b.Value.Length) return false;
				for (int i = 0; i < Value.Length; i++) {
					if (!Value[i].Equals(oth.Value[i])) return false;
				} return true;
			} return false;
		}


		public override string ToString() {
			if (Value.Length == 0) return "[]";
			string s = "[";
			foreach (Operand e in Value) {
				s += e.ToString() + ", ";
			}
			return s.Substring(0, s.Length - 2) + "]";
		}
	}
}
