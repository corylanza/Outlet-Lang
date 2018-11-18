using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Const : Operand {

		//null
		public Const() {
			Type = Primitive.Object;
			Value = null;
		}

		public Const(int value) {
			Type = Primitive.Int;
			Value = value;
		}

		public Const(string value) {
			Type = Primitive.String;
			Value = value;
		}

		public Const(float value) {
			Type = Primitive.Float;
			Value = value;
		}

		public Const(bool value) {
			Type = Primitive.Bool;
			Value = value;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override bool Equals(Operand b) => Value.Equals(b.Value);

		public override string ToString() => (Value ?? "null").ToString();
	}
}
