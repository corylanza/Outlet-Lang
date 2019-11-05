using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public abstract class Type : Operand<Type> {
		
		public Type() {
			Type = Primitive.MetaType;
		}

		public abstract bool Is(Type t);
		public abstract bool Is(Type t, out int level);
		public virtual dynamic Default() => null;

		private static Type ClosestAncestor(Type a, Type b) {
			if(!(a is Class && b is Class)) throw new NotImplementedException("common ancestor only currently works for classes");
			Class cur = a as Class;
			while(cur != Primitive.Object) {
				if(cur.Is(b) && b.Is(cur)) return cur;
				cur = cur.Parent;
			}
			cur = b as Class;
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