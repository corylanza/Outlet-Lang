using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {
	public enum Side { Left, Right }
	public enum Arity { Unary, Binary }

	public class Operator : IToken {

		//1: ++, --
		public static Operator Dot = new Operator(".", 1, Side.Left, (l, r) => l + r); // TODO
																					   //2: pre ++ and --, unary + and -, ~, &, sizeof
		public static Operator Negative = new Operator("-", 2, Side.Right, (l) => -l); //TODO
		public static Operator Not = new Operator("!", 2, Side.Right, (l) => !l); //TODO
		public static Operator Times = new Operator("*", 3, Side.Left, (l, r) => l * r);
		public static Operator Divide = new Operator("/", 3, Side.Left, (l, r) => l / r);
		public static Operator Modulus = new Operator("%", 3, Side.Left, (l, r) => l % r);
		public static Operator Plus = new Operator("+", 4, Side.Left, (l, r) => l + r);
		public static Operator Minus = new Operator("-", 4, Side.Left, (l, r) => l - r);
		//public static Operator LeftShift = new Operator("<<", 5, Side.Left, (l, r) => l << r);
		//public static Operator RightShift = new Operator(">>", 5, Side.Left, (l, r) => l >> r);
		public static Operator LT = new Operator("<", 6, Side.Left, (l, r) => l < r);
		public static Operator LTE = new Operator("<=", 6, Side.Left, (l, r) => l <= r);
		public static Operator GT = new Operator(">", 6, Side.Left, (l, r) => l > r);
		public static Operator GTE = new Operator(">=", 6, Side.Left, (l, r) => l >= r);
		public static Operator BoolEquals = new Operator("==", 7, Side.Left, (l, r) => l == r);
		public static Operator NotEqual = new Operator("!=", 7, Side.Left, (l, r) => l != r);
		//public static Operator BitwiseAnd = new Operator("&", 8, Side.Left, (l, r) => l & r);
		//public static Operator BitwiseXor = new Operator("^", 9, Side.Left, (l, r) => l ^ r);
		//public static Operator BitwiseOr = new Operator("|", 10, Side.Left, (l, r) => l | r);
		//public static Operator LogicalAnd = new Operator("&&", 11, Side.Left, (l, r) => l && r);
		//public static Operator LogicalOr = new Operator("||", 12, Side.Left, (l, r) => l || r);
		public static Operator Equal = new Operator("=", 14, Side.Right, (l, r) => { l.Value = r.Value; return l; }); //TODO from here on
		public static Operator PlusEqual = new Operator("+=", 14, Side.Right, (l, r) => l + r);
		public static Operator MinusEqual = new Operator("-=", 14, Side.Right, (l, r) => l + r);
		public static Operator DividedEqual = new Operator("/=", 14, Side.Right, (l, r) => l + r);
		public static Operator MultEqual = new Operator("*=", 14, Side.Right, (l, r) => l + r);
		public static Operator ModEqual = new Operator("%=", 14, Side.Right, (l, r) => l * r);

		public string Name;
		public int Precedence;
		public Side Asssoc;
		public Arity Arity;
		private Func<Operand, Operand, Operand> BinaryFunc;
		private Func<Operand, Operand> UnaryFunc;
		
		private Operator(string name, int precedence, Side associativity, Func<Operand, Operand, Operand> func) {
			Name = name;
			Precedence = precedence;
			Asssoc = associativity;
			BinaryFunc = func;
			Arity = Arity.Binary;
		}
		private Operator(string name, int precedence, Side associativity, Func<Operand, Operand> func) {
			Name = name;
			Precedence = precedence;
			Asssoc = associativity;
			UnaryFunc = func;
			Arity = Arity.Unary;
		}
		public Operand PerformOp(Operand l, Operand r) => BinaryFunc(l, r);
		public Operand PerformOp(Operand input) => UnaryFunc(input);
		public override string ToString() => Name;
	}


}
