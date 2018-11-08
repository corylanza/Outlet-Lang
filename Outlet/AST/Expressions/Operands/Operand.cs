using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public abstract class Operand : Expression {

		public dynamic Value;
		public Type Type;

		public override Operand Eval(Scope scope) => this;
		public override bool Equals(object obj) => obj is Operand o && Equals(o);
		public override int GetHashCode() => base.GetHashCode();
		public override void Resolve(Scope scope) {	}

		public override T Accept<T>(IVisitor<T> visitor) {
			throw new OutletException("unvisitable type, not part of AST");
		}

		public bool Cast(Type t) {
			if (Type.Is(t)) return true;
			throw new OutletException("cannot convert type " + Type.ToString() + " to type " + t.ToString());
		}
		
		public abstract bool Equals(Operand b);
		public abstract override string ToString();
	}

	public interface ICallable {
		Operand Call(params Operand[] args);
	}
	public interface IDereferenceable {
		Operand Dereference(string field);
	}
	public interface ICollection {
		Operand[] Values();
	}
	public interface IAssignable {
		void Assign(Scope s, Operand value);
	}
}
