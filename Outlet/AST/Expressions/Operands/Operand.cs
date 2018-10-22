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

		public override bool Equals(object obj) => obj is Operand o && Equals(o);
		public abstract bool Equals(Operand b);
		//public abstract Operand Dereference(FunctionCall method);
		public override int GetHashCode() => base.GetHashCode();

    }

	public interface ICallable {
		Operand Call(params Operand[] args);
	}
	public interface IDereferenceable {
		Operand Dereference(Identifier field);
	}
}
