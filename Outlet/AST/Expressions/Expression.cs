using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public abstract class Expression : Statement {

		public override void Execute() {
			Eval();
		}
		
		public abstract Operand Eval();
    }

    public abstract class Operand : Expression {
        public dynamic Value;

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
		public static Operand operator ==(Operand a, Operand b) => new Literal(a.Value == b.Value);
		public static Operand operator !=(Operand a, Operand b) => new Literal(a.Value != b.Value);

		public override bool Equals(object obj) => base.Equals(obj);
		public override int GetHashCode() =>  base.GetHashCode();
		
	}
}
