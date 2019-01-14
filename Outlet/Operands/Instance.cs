using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class Instance : Operand {
		
		public readonly Getter GetInstanceVar;
		public readonly Setter SetInstanceVar;

		public Instance(UserDefinedClass type, Getter get, Setter set) {
			Type = type;
			GetInstanceVar = get;
			SetInstanceVar = set;
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
