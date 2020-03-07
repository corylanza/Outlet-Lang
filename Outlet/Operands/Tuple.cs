using System.Linq;
using Outlet.Types;

namespace Outlet.Operands {
	public class OTuple : Operand<TupleType> {

        private readonly Operand[] Vals;

        public override TupleType RuntimeType { get; set; }//=> throw new System.NotImplementedException();

        public OTuple(params Operand[] vals) {
			Vals = vals;
			RuntimeType = new TupleType(vals.Select(val => val.GetOutletType()).ToArray());
		}

        public Operand[] Values() => Vals;

        public override bool Equals(Operand b) {
			if(b is OTuple oth) {
				if (Vals.Length != oth.Vals.Length) return false;
				for (int i = 0; i < Vals.Length; i++) {
					if (!Vals[i].Equals(oth.Vals[i])) return false;
			}
				return true;
			}
			return false;
		}

		public override string ToString() {
			string s = "(";
			foreach(Operand e in Vals) {
				s += e.ToString() + ", ";
			}
			return s.Substring(0, s.Length-2) + ")";
		}
	}
}
