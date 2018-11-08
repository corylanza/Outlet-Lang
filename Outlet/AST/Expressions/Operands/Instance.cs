using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Instance : Operand, IDereferenceable {
		
		//private readonly Dictionary<string, Operand> Fields = new Dictionary<string, Operand>();
		private readonly Scope Scope;

		public Instance(Class type, Scope closure) {
			Type = type;
			Scope = closure;
			Scope.Add("this", type, this);
		}

		public Operand Dereference(string field) {
			return Scope.Get(0, field);
		}

		public void Assign(string field, Operand o) {
			Scope.Assign(0, field, o);
		}

		public override bool Equals(Operand b) {
			return ReferenceEquals(this, b);
		}

		public override string ToString() {
			string s = Type.ToString()+" instance";
			return s;
		}
	}
}
