using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.Tokens {
	public enum Side { Left, Right }

	public abstract class Operator : Token {
		
		public static Operator PostInc =	new UnaryOperator ("++",   1,  Side.Left,  (l)	  =>	new Constant(l.Value++));
		public static Operator PostDec =	new UnaryOperator ("--",   1,  Side.Left,  (l)	  =>	new Constant(l.Value--));
		public static Operator Dot =		new BinaryOperator(".",    1,  Side.Left,  (l, r) =>	null);
		public static Operator PreInc =		new UnaryOperator ("++",   1,  Side.Left,  (l)	  =>	new Constant(++l.Value));
		public static Operator PreDec =		new UnaryOperator ("--",   1,  Side.Left,  (l)	  =>	new Constant(--l.Value));
		public static Operator UnaryPlus =  new UnaryOperator ("+",    2,  Side.Right, (l)	  =>	new Constant(+l.Value));
		public static Operator Complement = new UnaryOperator ("~",    2,  Side.Right, (l)	  =>	new Constant(~l.Value));
		public static Operator UnaryAnd =   new UnaryOperator ("&",    2,  Side.Right, (l)	  =>	null);
		public static Operator Negative =	new UnaryOperator ("-",    2,  Side.Right, (l)	  =>	new Constant(-l.Value));
		public static Operator Not =		new UnaryOperator ("!",    2,  Side.Right, (l)    =>	new Constant(!l.Value));
		public static Operator Times =		new BinaryOperator("*",    3,  Side.Left,  (l, r) =>	new Constant(l.Value * r.Value));
		public static Operator Divide =		new BinaryOperator("/",    3,  Side.Left,  (l, r) =>	new Constant(l.Value / r.Value));
		public static Operator Modulus =	new BinaryOperator("%",    3,  Side.Left,  (l, r) =>	new Constant(l.Value % r.Value));
		public static Operator Plus =		new BinaryOperator("+",    4,  Side.Left,  (l, r) =>	new Constant(l.Value + r.Value));
		public static Operator Minus =		new BinaryOperator("-",    4,  Side.Left,  (l, r) =>	new Constant(l.Value - r.Value));
		public static Operator LShift =		new BinaryOperator("<<",   5,  Side.Left,  (l, r) =>	new Constant(l.Value << r.Value));
		public static Operator RShift =		new BinaryOperator(">>",   5,  Side.Left,  (l, r) =>	new Constant(l.Value >> r.Value));
		public static Operator LT =			new BinaryOperator("<",    6,  Side.Left,  (l, r) =>	new Constant(l.Value < r.Value));
		public static Operator LTE =		new BinaryOperator("<=",   6,  Side.Left,  (l, r) =>	new Constant(l.Value <= r.Value));
		public static Operator GT =			new BinaryOperator(">",    6,  Side.Left,  (l, r) =>	new Constant(l.Value > r.Value));
		public static Operator GTE =		new BinaryOperator(">=",   6,  Side.Left,  (l, r) =>	new Constant(l.Value >= r.Value));
		public static Operator Is =			new BinaryOperator("is",   6,  Side.Left,  (l, r) =>	new Constant(l.Type.Is(r as AST.Type)));
		public static Operator Isnt =		new BinaryOperator("isnt", 6,  Side.Left,  (l, r) =>	new Constant(!l.Type.Is(r as AST.Type)));
		public static Operator BoolEquals = new BinaryOperator("==",   7,  Side.Left,  (l, r) =>	new Constant(l.Equals(r)));
		public static Operator NotEqual =	new BinaryOperator("!=",   7,  Side.Left,  (l, r) =>	new Constant(!l.Equals(r)));
		public static Operator BitAnd =		new BinaryOperator("&",    8,  Side.Left,  (l, r) =>	new Constant(l.Value & r.Value));
		public static Operator BitXor =		new BinaryOperator("^",    9,  Side.Left,  (l, r) =>	new Constant(l.Value ^ r.Value));
		public static Operator BitOr =		new BinaryOperator("|",    10, Side.Left,  (l, r) =>	new Constant(l.Value | r.Value));
		public static Operator LogicalAnd = new BinaryOperator("&&",   11, Side.Left,  (l, r) =>	null);
		public static Operator LogicalOr =	new BinaryOperator("||",   12, Side.Left,  (l, r) =>	null);
		public static Operator Question =	new BinaryOperator("?",    13, Side.Right, (l, r) =>	null);
		public static Operator Ternary =	new BinaryOperator(":",    13, Side.Right, (l, r) =>	null);
		public static Operator Lambda =		new BinaryOperator("=>",   14, Side.Right, (l, r) =>	null);
		public static Operator Equal =		new BinaryOperator("=",    14, Side.Right, (l, r) =>	null);
		public static Operator PlusEqual =	new BinaryOperator("+=",   14, Side.Right, (l, r) =>	null);
		public static Operator MinusEqual = new BinaryOperator("-=",   14, Side.Right, (l, r) =>	null);
		public static Operator DivEqual =	new BinaryOperator("/=",   14, Side.Right, (l, r) =>	null);
		public static Operator MultEqual =	new BinaryOperator("*=",   14, Side.Right, (l, r) =>	null);
		public static Operator ModEqual =	new BinaryOperator("%=",   14, Side.Right, (l, r) =>	null);

		public readonly string Name;
		public readonly int Precedence;
		public readonly Side Asssoc;

		protected Operator(string name, int precedence, Side associativity) {
			Name = name;
			Precedence = precedence;
			Asssoc = associativity;
		}
		public override string ToString() => Name;
	}

	public class BinaryOperator : Operator {

		private readonly Func<Operand, Operand, Operand> BinaryFunc;

		public BinaryOperator(string name, int p, Side a, Func<Operand, Operand, Operand> func) : base(name, p, a) {
			BinaryFunc = func;
		}
		
		public Operand PerformOp(Operand l, Operand r) => BinaryFunc(l, r);

		// Right param is first due to shunting yard popping the right operand first
		public Expression Construct(Expression r, Expression l) {
			if (this == Dot) return new Deref(l, r);
			if (this == Lambda) return new Lambda(l, r);
			if (this == Equal) return new Assign(l, r);
			if (this == LogicalAnd || this == LogicalOr) return new ShortCircuit(l, this, r);
			return new Binary(l, this, r);
		}
	}

	public class UnaryOperator : Operator {

		private readonly Func<Operand, Operand> UnaryFunc;

		public UnaryOperator(string name, int p, Side a, Func<Operand, Operand> func) : base(name, p, a) {
			UnaryFunc = func;
		}

		public Operand PerformOp(Operand e) => UnaryFunc(e);
	}

}
