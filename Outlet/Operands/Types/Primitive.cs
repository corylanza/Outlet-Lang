using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class Primitive : Type {
		
		public static Primitive MetaType = new Primitive("type", null, null);
		public static Primitive Void = new Primitive("void", null, null);
		public static Primitive Object = new Primitive("object", null, null);
		public static Primitive Float = new Primitive("float", Object, 0.0f);
		public static Primitive Int = new Primitive("int", Float, 0);
		public static Primitive Bool = new Primitive("bool", Object, false);
		public static Primitive String = new Primitive("string", Object, "");
		// cannot be referenced like other types but is used under the hood
		public static Primitive Null = new Primitive("null", Object, null);

		private readonly string Name;

		private Primitive(string name, Type parent, object def) : base(parent, def) {
			Name = name;
		}

		public override bool Is(Type t) {
			if(Equals(t)) return true;
			if(!(Parent is null)) return Parent.Is(t);
			return false;
		}

		public override bool Is(Type t, out int level) {
			level = 0;
			if(Equals(t)) return true;
			if(!(Parent is null) && Parent.Is(t, out int l)) {
				level = l+1;
				return true;
			}
			level = -1;
			return false;
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override string ToString() => Name;
	}
}
