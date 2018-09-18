﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Expressions {
    public abstract class Expression {

        public abstract Operand Eval();

        public abstract override string ToString();
    }

    public abstract class Operand : Expression {
        public dynamic Value;

        public override Operand Eval() => this;

		public override string ToString() => Value.ToString();


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
	}
}
