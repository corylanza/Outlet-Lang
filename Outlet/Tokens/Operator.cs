using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet.Tokens {
	public enum Side { Left, Right }

	public abstract class Operator : Token {

		public static readonly UnaryOperator PostInc, PostDec, PreInc, PreDec, UnaryPlus, Complement, UnaryAnd, Negative, Not;
		public static readonly BinaryOperator Dot, Times, Divide, Modulus, Plus, Minus, LShift, RShift,
											  LT, LTE, GT, GTE, Is, Isnt, BoolEquals, NotEqual, BitAnd,
											  BitXor, BitOr, LogicalAnd, LogicalOr, Question, Ternary,
											  Lambda, Equal, PlusEqual, MinusEqual, DivEqual, MultEqual, ModEqual;

		static Operator() {
			Type Int = Primitive.Int;
			Type Flt = Primitive.Float;
			Type Str = Primitive.String;
			Type Obj = Primitive.Object;
			Type Bln = Primitive.Bool;
			Type Met = Primitive.MetaType;


			PostInc =		new UnaryOperator("++",    1,  Side.Left,	new UnOp(Int, Int, (l) => new Constant(l.Value++)));
			PostDec =		new UnaryOperator("--",    1,  Side.Left,	new UnOp(Int, Int, (l) => new Constant(l.Value--)));
			Lambda =		new BinaryOperator("=>",   1,  Side.Left);
			Dot =			new BinaryOperator(".",    1,  Side.Left);
			PreInc =		new UnaryOperator("++",	   1,  Side.Left,	new UnOp(Int, Int, (l) => new Constant(++l.Value)));
			PreDec =		new UnaryOperator("--",	   1,  Side.Left,	new UnOp(Int, Int, (l) => new Constant(--l.Value)));
			UnaryPlus =		new UnaryOperator("+",	   2,  Side.Right);
			Complement =	new UnaryOperator("~",	   2,  Side.Right,	new UnOp(Int, Int, (l) => new Constant(~l.Value)));
			UnaryAnd =		new UnaryOperator("&",	   2,  Side.Right,	new UnOp(Obj, Met, (l) => l.Type));
			Negative =		new UnaryOperator("-",	   2,  Side.Right,	new UnOp(Int, Int, (l) => new Constant(-l.Value)), 
																		new UnOp(Flt, Flt, (l) => new Constant(-l.Value)),
																		new UnOp(Str, Str, (l) => new Constant("olleh")));
			Not =			new UnaryOperator("!",	   2,  Side.Right,	new UnOp(Bln, Bln, (l) => new Constant(!l.Value)));
			Times =			new BinaryOperator("*",    3,  Side.Left,	new BinOp(Int, Int, Int, (l, r) => new Constant(l.Value * r.Value)),
																		new BinOp(Flt, Flt, Flt, (l, r) => new Constant(l.Value * r.Value)));
			Divide =		new BinaryOperator("/",    3,  Side.Left, 	new BinOp(Int, Int, Int, (l, r) => r.Value == 0 ? 
																			throw new RuntimeException("Divide By 0") : 
																			new Constant(l.Value / r.Value)),
																		new BinOp(Flt, Flt, Flt, (l, r) => r.Value == 0 ?
																			throw new RuntimeException("Divide By 0") :
																			new Constant(l.Value / r.Value)));
			Modulus =		new BinaryOperator("%",    3,  Side.Left, 	new BinOp(Int, Int, Int, (l, r) => r.Value == 0 ? 
																			throw new RuntimeException("Divide By 0") : 
																			new Constant(l.Value % r.Value)));
			Plus =			new BinaryOperator("+",    4,  Side.Left, 	new BinOp(Int, Int, Int, (l, r) => new Constant(l.Value + r.Value)),
																		new BinOp(Flt, Flt, Flt, (l, r) => new Constant(l.Value + r.Value)),
																		new BinOp(Str, Obj, Str, (l, r) => new Constant(l.Value + r.ToString())),
																		new BinOp(Obj, Str, Str, (l, r) => new Constant(l.ToString() + r.Value)));
			Minus =			new BinaryOperator("-",    4,  Side.Left, 	new BinOp(Int, Int, Int, (l, r) => new Constant(l.Value - r.Value)),
																		new BinOp(Flt, Flt, Flt, (l, r) => new Constant(l.Value - r.Value)));
			LShift =		new BinaryOperator("<<",   5,  Side.Left, 	new BinOp(Int, Int, Int, (l, r) => new Constant(l.Value << r.Value)));
			RShift =		new BinaryOperator(">>",   5,  Side.Left, 	new BinOp(Int, Int, Int, (l, r) => new Constant(l.Value >> r.Value)));
			LT =			new BinaryOperator("<",    6,  Side.Left,	new BinOp(Flt, Flt, Bln, (l, r) => new Constant(l.Value < r.Value)));
			LTE =			new BinaryOperator("<=",   6,  Side.Left,  	new BinOp(Flt, Flt, Bln, (l, r) => new Constant(l.Value <= r.Value)));
			GT =			new BinaryOperator(">",    6,  Side.Left, 	new BinOp(Flt, Flt, Bln, (l, r) => new Constant(l.Value > r.Value)));
			GTE =			new BinaryOperator(">=",   6,  Side.Left, 	new BinOp(Flt, Flt, Bln, (l, r) => new Constant(l.Value >= r.Value)));
			Is =			new BinaryOperator("is",   6,  Side.Left);
			Isnt =			new BinaryOperator("isnt", 6,  Side.Left);
			BoolEquals =	new BinaryOperator("==",   7,  Side.Left, 	new BinOp(Obj, Obj, Bln, (l, r) => new Constant(l.Equals(r))));
			NotEqual =		new BinaryOperator("!=",   7,  Side.Left, 	new BinOp(Obj, Obj, Bln, (l, r) => new Constant(!l.Equals(r))));
			BitAnd =		new BinaryOperator("&",	   8,  Side.Left,	new BinOp(Int, Int, Int, (l, r) => new Constant(l.Value & r.Value)));
			BitXor =		new BinaryOperator("^",    9,  Side.Left,  	new BinOp(Int, Int, Int, (l, r) => new Constant(l.Value ^ r.Value)));
			BitOr =			new BinaryOperator("|",    10, Side.Left,  	new BinOp(Int, Int, Int, (l, r) => new Constant(l.Value | r.Value)));
			LogicalAnd =	new BinaryOperator("&&",   11, Side.Left);
			LogicalOr =		new BinaryOperator("||",   12, Side.Left);
			Question =		new BinaryOperator("?",    13, Side.Right);
			Ternary =		new BinaryOperator(":",    13, Side.Right);
			Equal =			new BinaryOperator("=",    14, Side.Right);
			PlusEqual =		new BinaryOperator("+=",   14, Side.Right);
			MinusEqual =	new BinaryOperator("-=",   14, Side.Right);
			DivEqual =		new BinaryOperator("/=",   14, Side.Right);
			MultEqual =		new BinaryOperator("*=",   14, Side.Right);
			ModEqual =		new BinaryOperator("%=",   14, Side.Right);
		}

		public readonly string Name;
		public readonly int Precedence;
		public readonly Side Assoc;

		protected Operator(string name, int precedence, Side associativity) => 
			(Name, Precedence, Assoc) = (name, precedence, associativity);

		public override string ToString() => Name;
	}

	public class BinaryOperator : Operator {

		public Overload<BinOp> Overloads;

		public BinaryOperator(string name, int p, Side a, params BinOp[] defaultoverloads) : base(name, p, a) {
			Overloads = new Overload<BinOp>(defaultoverloads);
		}

		// Right param is first due to shunting yard popping the right operand first
		public Expression Construct(Expression r, Expression l) {
			if (this == Is || this == Isnt) return new Is(l, r, this == Is);
			if (this == Dot) return new Deref(l, r);
			if (this == Lambda) return new Lambda(l, r);
			if (this == Equal) return new Assign(l, r);
			if (this == LogicalAnd || this == LogicalOr) return new ShortCircuit(l, this, r);
			return new Binary(Name, l, r, Overloads);
		}
	}

	public class UnaryOperator : Operator {

		public Overload<UnOp> Overloads;

		public UnaryOperator(string name, int p, Side a, params UnOp[] overloads) : base(name, p, a) {
			Overloads = new Overload<UnOp>(overloads);
		}
	}

}
