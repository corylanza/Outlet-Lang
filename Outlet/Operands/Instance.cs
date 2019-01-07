using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class Instance : Operand {
		
		//private readonly Scope Scope;
		public readonly StaticFunc SF;

		public Instance(Class type, StaticFunc sf) {
			Type = type;
			//Scope = closure;
			SF = sf;
			//Scope.Add("this", type, this);
		}
		/*
		public Operand Dereference(string field) {
			return Scope.Get(0, field);
		}

		public void Assign(string field, Operand o) {
			Scope.Assign(0, field, o);
		}*/

		public override bool Equals(Operand b) {
			return ReferenceEquals(this, b);
		}

		public override string ToString() {
			string s = Type.ToString()+" instance";
			return s;
		}
	}
}
