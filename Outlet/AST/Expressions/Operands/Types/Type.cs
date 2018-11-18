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

		public abstract bool Is(Type t);

		public abstract bool Is(Type t, out int level);

		/*
		public bool Is(Type t) {
			if (Equals(t)) return true;
			if (!(Parent is null)) return Parent.Is(t);
			return false;
		}*/

		public abstract Operand Dereference(string field);

		//public override void Resolve(Scope scope) {	}
	}

}