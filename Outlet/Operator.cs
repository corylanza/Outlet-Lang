using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Expressions;

namespace Outlet {
	public enum Side { Left, Right }

	public class Operator : IToken {

		public static Operator Dot = new Operator(".", 1, Side.Left, (l, r) => l + r); // TODO
		public static Operator Not = new Operator("!", 2, Side.Right, (l, r) => l + r); //TODO
		public static Operator Times = new Operator("*", 3, Side.Left, (l, r) => l * r);
		public static Operator Divide = new Operator("/", 3, Side.Left, (l, r) => l / r);
		public static Operator Modulus = new Operator("%", 3, Side.Left, (l, r) => l % r);
		public static Operator Plus = new Operator("+", 4, Side.Left, (l, r) => l + r);
		public static Operator Minus = new Operator("-", 4, Side.Left, (l, r) => l - r);
		public static Operator LT = new Operator("<", 6, Side.Left, (l, r) => l < r);
		public static Operator LTE = new Operator("<=", 6, Side.Left, (l, r) => l <= r);
		public static Operator GT = new Operator(">", 6, Side.Left, (l, r) => l > r);
		public static Operator GTE = new Operator(">=", 6, Side.Left, (l, r) => l >= r);
		public static Operator BoolEquals = new Operator("==", 7, Side.Left, (l, r) => l == r);
		public static Operator NotEqual = new Operator("!=", 7, Side.Left, (l, r) => l != r);
		public static Operator Equal = new Operator("=", 14, Side.Right, (l, r) => l + r); //TODO from here on
		public static Operator PlusEqual = new Operator("+=", 14, Side.Right, (l, r) => l + r);
		public static Operator MinusEqual = new Operator("-=", 14, Side.Right, (l, r) => l + r);
		public static Operator DividedEqual = new Operator("/=", 14, Side.Right, (l, r) => l + r);
		public static Operator MultEqual = new Operator("*=", 14, Side.Right, (l, r) => l + r);
		public static Operator ModEqual = new Operator("%=", 14, Side.Right, (l, r) => l * r);

		public string Name;
		public int Precedence;
		public Side Asssoc;
		private Func<Operand, Operand, Operand> Function;
		private Operator(string name, int precedence, Side associativity, Func<Operand, Operand, Operand> func) {
			Name = name;
			Precedence = precedence;
			Asssoc = associativity;
			Function = func;
		}
		public Operand PerformOp(Operand l, Operand r) => Function(l, r);
		public override string ToString() => Name;
	}


}
