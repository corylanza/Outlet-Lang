using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class Primitive : Class {
		
		public static readonly Primitive MetaType = new Primitive("type", null, null);
		public static readonly Primitive Void = new Primitive("void", null, null);
		public static readonly Primitive Object = new Primitive("object", null, null);
		public static readonly Primitive Float = new Primitive("float", Object, 0.0f);
		public static readonly Primitive Int = new Primitive("int", Float, 0);
		public static readonly Primitive Bool = new Primitive("bool", Object, false);
		public static readonly Primitive String = new Primitive("string", Object, "");
		// cannot be referenced like other types but is used under the hood
		public static readonly Primitive Null = new Primitive("null", Object, null);
        
		private Primitive(string name, Class parent, object def) : base(name, parent, def) {
			Name = name;
		}
	}
}
