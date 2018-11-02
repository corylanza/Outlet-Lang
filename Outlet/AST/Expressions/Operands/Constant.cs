using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Constant : Operand {

		//null
		public Constant() {
			Type = Primitive.Object;
			Value = null;
		}

		public Constant(int value) {
			Type = Primitive.Int;
			Value = value;
		}

		public Constant(string value) {
			Type = Primitive.String;
			Value = value;
		}

		public Constant(float value) {
			Type = Primitive.Float;
			Value = value;
		}

		public Constant(bool value) {
			Type = Primitive.Bool;
			Value = value;
		}

		public override bool Equals(Operand b) => Value.Equals(b.Value);

		public override string ToString() => (Value ?? "null").ToString();
	}
}
