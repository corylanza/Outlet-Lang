using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public abstract class Operand : Expression {

		public dynamic Value;
		public Type Type;

		public static Operand operator -(Operand a) => new Literal(-a.Value);
		public static Operand operator !(Operand a) => new Literal(!a.Value);
		public static Operand operator *(Operand a, Operand b) => new Literal(a.Value * b.Value);
		public static Operand operator /(Operand a, Operand b) => new Literal(a.Value / b.Value);
		public static Operand operator %(Operand a, Operand b) => new Literal(a.Value % b.Value);
		public static Operand operator +(Operand a, Operand b) => new Literal(a.Value + b.Value);
		public static Operand operator -(Operand a, Operand b) => new Literal(a.Value - b.Value);
		public static Operand operator <(Operand a, Operand b) => new Literal(a.Value < b.Value);
		public static Operand operator >(Operand a, Operand b) => new Literal(a.Value > b.Value);
		public static Operand operator <=(Operand a, Operand b) => new Literal(a.Value <= b.Value);
		public static Operand operator >=(Operand a, Operand b) => new Literal(a.Value >= b.Value);
		public static Operand operator ==(Operand a, Operand b) => new Literal(a.Equals(b));
		public static Operand operator !=(Operand a, Operand b) => new Literal(!a.Equals(b));

		public override Operand Eval(Scope scope) => this;
		public override bool Equals(object obj) => obj is Operand o && Equals(o);
		public override int GetHashCode() => base.GetHashCode();
		public override void Resolve(Scope scope) {	}

		public bool Cast(Type t) {
			if (Type.Is(t)) return true;
			throw new OutletException("cannot convert type " + Type.ToString() + " to type " + t.ToString());
		}

		//public abstract dynamic NewValue { get; set; }
		public abstract bool Equals(Operand b);
		public abstract override string ToString();
	}

	public interface ICallable {
		Operand Call(params Operand[] args);
	}
	public interface IDereferenceable {
		Operand Dereference(Identifier field);
	}
	public interface ICollection {
		Operand[] Values();
	}
}
