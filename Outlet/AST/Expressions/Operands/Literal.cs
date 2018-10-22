using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Literal : Operand, IToken {
		public Literal(int value) {
			Type = Primitive.Int;
			Value = value;
		}

		public Literal(string value) {
			Type = Primitive.String;
			Value = value;
		}

		public Literal(float value) {
			Type = Primitive.Float;
			Value = value;
		}

		public Literal(bool value) {
			Type = Primitive.Bool;
			Value = value;
		}

		public override Operand Eval(Scope block) => this;

		public override bool Equals(Operand b) => Value.Equals(b.Value);

		public override string ToString() => Value.ToString();

		public override void Resolve(Scope block) { }
	}
}
