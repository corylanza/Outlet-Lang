using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class OTuple : Operand<TupleType> {

		public OTuple(params Operand[] vals) {
			Value = vals;
			Type = new TupleType(vals.Select(val => val.GetOutletType()).ToArray());
		}

		public override bool Equals(Operand b) {
			if(b is OTuple oth) {
				if (Value.Length != b.Value.Length) return false;
				for (int i = 0; i < Value.Length; i++) {
					if (!Value[i].Equals(oth.Value[i])) return false;
			}
				return true;
			}
			return false;
		}

		public override string ToString() {
			string s = "(";
			foreach(Operand e in Value) {
				s += e.ToString() + ", ";
			}
			return s.Substring(0, s.Length-2) + ")";
		}
	}
}
