using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decls = System.Collections.Generic.Dictionary<string, Outlet.Operands.Type>;

namespace Outlet.Operands {
	public class ProtoClass : Class {

		public readonly Decls Statics;
		public readonly Decls Instances;

		public ProtoClass(string name, Decls instances, Decls statics) : base(name, null) {
			Instances = instances;
			Statics = statics;
		}

	}
}
