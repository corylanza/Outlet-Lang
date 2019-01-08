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

		private static Type ClosestAncestor(Type a, Type b) {
			Type cur = a;
			while(cur != Primitive.Object) {
				if(cur.Is(b) && b.Is(cur)) return cur;
				cur = cur.Parent;
			}
			cur = b;
			while(cur != Primitive.Object) {
				if(cur.Is(a) && a.Is(cur)) return cur;
				cur = cur.Parent;
			}
			return Primitive.Object;
		}

		public static Type CommonAncestor(params Type[] types) {
			if(types.Length == 0) return Primitive.Object;
			Type ancestor = types[0];
			foreach(Type cur in types) {
				ancestor = ClosestAncestor(ancestor, cur);
			}
			return ancestor;
		}
		
	}

}