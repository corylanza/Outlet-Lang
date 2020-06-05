using System;
using Outlet.Operands;
using String = Outlet.Operands.String;

namespace Outlet.Types {
	public class Primitive : Class {
		
		public static readonly Primitive MetaType = new Primitive("type", null);
		public static readonly Primitive Void = new Primitive("void", null);
		public static readonly Primitive Object = new Primitive("object", null);
		public static readonly Primitive Float = new Primitive("float", Object, () => Value.Float(0.0f));
		public static readonly Primitive Int = new Primitive("int", Float, () => Value.Int(0));
		public static readonly Primitive Bool = new Primitive("bool", Object, () => Value.Bool(false));
		public static readonly Primitive String = new Primitive("string", Object, () => new String(""));

        private readonly Func<Operand> Initialize;


		private Primitive(string name, Class parent, Func<Operand> def) : base(name, parent) {
            Initialize = def;
		}

        private Primitive(string name, Class? parent) : base(name, parent) { Initialize = base.Default; }

        public override Operand Default() => Initialize();

    }
}
