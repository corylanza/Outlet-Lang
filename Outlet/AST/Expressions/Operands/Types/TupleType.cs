using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class TupleType : Type {

		public readonly Type[] Types;

		public TupleType(params Type[] types) : base(Primitive.Object, null) {
			Types = types;
		}

		public override Operand Dereference(string field) {
			throw new NotImplementedException();
		}

		public override bool Equals(Operand b) {
			if(b is TupleType t && t.Types.Length == Types.Length) {
				for(int i = 0; i < Types.Length; i++) {
					if (!Types[i].Equals(t.Types[i])) return false;
				} return true;
			} return false;
		}

		public override string ToString() {
			string s = "(";
			foreach (Expression e in Types) {
				s += e.ToString() + ", ";
			}
			return s.Substring(0, s.Length - 2) + ")";
		}
	}
}
