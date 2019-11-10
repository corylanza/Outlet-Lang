using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class Primitive : Class {
		
		public static readonly Primitive MetaType = new Primitive("type", null);
		public static readonly Primitive Void = new Primitive("void", null);
		public static readonly Primitive Object = new Primitive("object", null);
		public static readonly Primitive Float = new Primitive("float", Object, () => Constant.Float(0.0f));
		public static readonly Primitive Int = new Primitive("int", Float, () => Constant.Int(0));
		public static readonly Primitive Bool = new Primitive("bool", Object, () => Constant.Bool(false));
		public static readonly Primitive String = new Primitive("string", Object, () => Constant.String(""));

        private readonly Func<Operand> Initialize;


		private Primitive(string name, Class parent, Func<Operand> def) : base(name, parent) {
            Initialize = def;
		}

        private Primitive(string name, Class parent) : base(name, parent) { Initialize = base.Default; }

        public override Operand Default() => Initialize();

        public new enum Type
        {
            Object,
            Type,
            Void,
            Float,
            Int,
            Bool,
            String
        }
	}
}
