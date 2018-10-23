using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public abstract class Type : Operand, IDereferenceable {

		public static Type List = new Class("list", new List<Identifier>());

		public readonly string Name;
		public readonly Type Parent;
		public readonly object Default;

		public Type(string name, Type parent, object def) {
			Name = name;
			Parent = parent;
			Default = def;
		}

		public bool Is(Type t) {
			if (ReferenceEquals(this, t)) return true;
			if (!(Parent is null)) return Parent.Is(t);
			return false;
		}

		public static Type Construct(Operand o) {
			if (o is Literal l) return l.Type;
			//if (o is OTuple t) return TupleType(t.Value.Select(x => Construct(x.Type)))
			return null;
		}

		public override void Resolve(Scope scope) {
			throw new NotImplementedException("resolving not implemented for types");
		}

		public Operand Dereference(Identifier field) {
			throw new NotImplementedException();
		}
	}

}