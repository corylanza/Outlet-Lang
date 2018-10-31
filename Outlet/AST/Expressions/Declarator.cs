using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Declarator : Expression {

		public readonly Expression Type;
		public readonly string ID;

		public Declarator(Expression type, Identifier id) {
			Type = type;
			ID = id.Name;
		}

		public Type GetType(Scope scope) {
			Operand t = Type.Eval(scope);
			if (t is Type type) return type;
			else throw new OutletException(Type.ToString() + " is not a valid type");
		}

		public override Operand Eval(Scope scope) {
			throw new OutletException("invalid use of Declarator, THIS SHOULD NEVER PRINT");
		}

		public override void Resolve(Scope scope) {
			Type.Resolve(scope);
		}

		public override string ToString() => Type.ToString() + " " + ID;
	}
}
