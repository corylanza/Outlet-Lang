using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class TupleType : Type {

		public readonly Type[] Types;

		public TupleType(params Type[] types) : base(Primitive.Object, null) {
			Types = types;
		}

		public override bool Is(Type t) {
			if(t == Primitive.Object) return true;
			if(t is TupleType tt && tt.Types.Length == Types.Length){
				for(int i = 0; i < Types.Length; i++) {
					if(!Types[i].Is(tt.Types[i])) return false;
				}
				return true;
			} return false;
		}

		public override bool Is(Type t, out int level) {
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
			foreach (Type t in Types) {
				s += t.ToString() + ", ";
			}
			if(s == "(") return "()";
			return s.Substring(0, s.Length - 2) + ")";
		}
	}
}
