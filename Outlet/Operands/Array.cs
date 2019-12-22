using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class Array : Operand<ArrayType> {

        private readonly Operand[] Vals;

		public Array(params Operand[] vals) {
            RuntimeType = new ArrayType(Type.CommonAncestor(vals.Select(x => x.GetOutletType()).ToArray()));
			Vals = vals;
		}

        public Operand[] Values() => Vals;

		public override bool Equals(Operand b) {
			if (b is Array oth) {
				if(Vals.Length != oth.Vals.Length) return false;
				for (int i = 0; i < Vals.Length; i++) {
					if (!Vals[i].Equals(oth.Vals[i])) return false;
				} return true;
			} return false;
		}


		public override string ToString() {
			if (Vals.Length == 0) return "[]";
			string s = "[";
			foreach (Operand e in Vals) {
				s += e.ToString() + ", ";
			}
			return s.Substring(0, s.Length - 2) + "]";
		}
    }
}
