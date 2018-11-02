using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public abstract class Type : Operand, IDereferenceable {

		public readonly Type Parent;
		public readonly object Default;

		public Type(Type parent, object def) {
			Parent = parent;
			Default = def;
			Type = Primitive.MetaType;
		}

		// this must be in each class
		public bool Is(Type t) {
			if (Equals(t)) return true;
			if (!(Parent is null)) return Parent.Is(t);
			return false;
		}

		public abstract Operand Dereference(string field);

		public static Type Construct(Operand o) {
			if (o is Constant l) return l.Type;
			//if (o is OTuple t) return new TupleType(t.Value.Select(x => Construct(x.Type)))
			return null;
		}

		public override void Resolve(Scope scope) {	}
	}

}