using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class TupleType : Type {

		private readonly Type[] Types;

		public TupleType(params Type[] types) : base("", Primitive.Object, null) {
			Types = types;
		}

		public override Operand Dereference(Identifier feld) {
			throw new NotImplementedException();
		}

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
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
