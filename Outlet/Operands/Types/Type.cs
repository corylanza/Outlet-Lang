using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public abstract class Type : Operand {

		public readonly Type Parent;
		public readonly dynamic Default;

		public Type(Type parent, object def) {
			Parent = parent;
			Default = def;
			Type = Primitive.MetaType;
		}

		public abstract bool Is(Type t);
		public abstract bool Is(Type t, out int level);

		public static Type CommonAncestor(Type a, Type b) {
			if(a.Is(b) && b.Is(a)) return a;
			return Primitive.Object;
		}
		
	}

}