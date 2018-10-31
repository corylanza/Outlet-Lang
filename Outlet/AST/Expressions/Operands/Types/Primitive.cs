using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Primitive : Type {
		
		public static Primitive MetaType = new Primitive("type", null, null);
		public static Primitive FuncType = new Primitive("func", null, null);
		public static Primitive Void = new Primitive("void", null, null);
		public static Primitive Object = new Primitive("object", null, null);
		public static Primitive Int = new Primitive("int", Object, 0);
		public static Primitive Float = new Primitive("float", Object, 0.0f);
		public static Primitive Bool = new Primitive("bool", Object, false);
		public static Primitive String = new Primitive("string", Object, "");
		public static Primitive List = new Primitive("list", Object, "");

		private Primitive(string name, Type parent, object def) : base(name, parent, def) {	}

		public override Operand Dereference(Identifier feld) {
			throw new NotImplementedException();
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override string ToString() => Name;
	}
}
