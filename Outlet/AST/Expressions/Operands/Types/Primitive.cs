using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Primitive : Type, IToken {

		public static Primitive Object = new Primitive("object", null, null);
		public static Primitive Int = new Primitive("int", Object, 0);
		public static Primitive Float = new Primitive("float", Object, 0.0f);
		public static Primitive Bool = new Primitive("bool", Object, false);
		public static Primitive String = new Primitive("string", Object, "");

		private Primitive(string name, Type parent, object def) : base(name, parent, def) {	}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override Operand Eval(Scope scope) => this;

		public override string ToString() => Name;
	}
}
