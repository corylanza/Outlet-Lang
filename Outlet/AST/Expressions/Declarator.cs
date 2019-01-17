using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Declarator : Expression {

		public readonly Expression Type;
		public readonly string ID;

		public Declarator(Expression type, string id) {
			Type = type;
			ID = id;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Type.ToString() + " " + ID;
	}
}
