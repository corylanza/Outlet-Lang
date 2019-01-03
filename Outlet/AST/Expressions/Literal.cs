using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;

namespace Outlet.AST {
	public class Literal : Expression {

		public Operands.Type Type;
		public dynamic Value;

		public Literal() {
			Type = Primitive.Object;
			Value = null;
		}

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

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => (Value ?? "null").ToString();
	}
}
