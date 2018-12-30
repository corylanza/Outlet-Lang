using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Array : Operand, ICallable, IDereferenceable, ICollection {
		public Array(params Operand[] vals) {
			Type = new ArrayType(Primitive.Object);
			Value = vals;
		}

		public Operand Dereference(string field) {
			if (field == "length") return new Const(Value.Length);
			throw new OutletException("field "+field+" not defined");
		}

		public Operand Call(params Operand[] args) => Value[args[0].Value];

		public Operand[] Values() => Value;

		public override bool Equals(Operand b) {
			if (b is Array oth) {
				if (Value.Length != b.Value.Length) return false;
				for (int i = 0; i < Value.Length; i++) {
					if (!Value[i].Equals(oth.Value[i])) return false;
				}
				return true;
			}
			return false;
		}

		public static Array operator +(Array a, Array b) {
			return new Array(a.Values().Union(b.Values()).ToArray());
		}

		public override string ToString() {
			if (Value.Length == 0) return "[]";
			 string s = "[";
			foreach (Expression e in Value) {
				s += e.ToString() + ", ";
			}
			return s.Substring(0, s.Length - 2) + "]";
		}
	}
}
