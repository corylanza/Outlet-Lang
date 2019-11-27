using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class Instance : Operand<Class> {
		
		public readonly Getter GetInstanceVar;
		public readonly Setter SetInstanceVar;
        public readonly Lister GetInstanceVars;

		public Instance(Class type, Getter get, Setter set, Lister list) {
			Type = type;
			GetInstanceVar = get;
			SetInstanceVar = set;
            GetInstanceVars = list;
		}

		public override bool Equals(Operand b) {
			return ReferenceEquals(this, b);
		}

		public override string ToString() {
			string s = Type.ToString()+" {\n";
            foreach(var (id, val) in GetInstanceVars())
            {
                s += "\t" + id + ": " + val.ToString() + "\n";
            }
			return s + "}";
		}
	}
}
