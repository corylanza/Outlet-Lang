using System.Linq;
using Outlet.AST;
using Outlet.Operands;
using Outlet.Types;
using Int = Outlet.Operands.Value<int>;
using Bln = Outlet.Operands.Value<bool>;
using Flt = Outlet.Operands.Value<float>;
using Str = Outlet.Operands.String;
using Obj = Outlet.Operands.Operand;

namespace Outlet.Tokens {
	public enum Side { Left, Right }

	public abstract class Operator : Token {

		public static readonly UnaryOperator PostInc, PostDec, PreInc, PreDec, UnaryPlus, Complement, UnaryAnd, Negative, Not;
		public static readonly BinaryOperator Dot, Times, Divide, Modulus, Plus, Minus, LShift, RShift,
											  LT, LTE, GT, GTE, Is, As, Isnt, BoolEquals, NotEqual, BitAnd,
											  BitXor, BitOr, LogicalAnd, LogicalOr, Question, Ternary,
											  Lambda, Equal, PlusEqual, MinusEqual, DivEqual, MultEqual,
											  /*IncRange, ExcRange,*/ ModEqual;

		static Operator() {


			PostInc =		new UnaryOperator("++",    1,  Side.Left);
			PostDec =		new UnaryOperator("--",    1,  Side.Left);
			Lambda =		new BinaryOperator("=>",   1,  Side.Left);
			Dot =			new BinaryOperator(".",    1,  Side.Left);
			//ExcRange =		new BinaryOperator("..",   1,  Side.Right,   new BinOp<Int, Int, Operands.Array>((l, r) => Range(l.Val, r.Val, false)));
			//IncRange =		new BinaryOperator("...",  1,  Side.Right,   new BinOp<Int, Int, Operands.Array>((l, r) => Range(l.Val, r.Val, true)));
			PreInc =		new UnaryOperator("++",	   1,  Side.Left);
			PreDec =		new UnaryOperator("--",	   1,  Side.Left);
			UnaryPlus =		new UnaryOperator("+",	   2,  Side.Right);
			Complement =	new UnaryOperator("~",	   2,  Side.Right,	new UnOp<Int, Int>((l) => Value.Int(~l.Underlying)));
			UnaryAnd =		new UnaryOperator("&",	   2,  Side.Right,	new UnOp<Obj, TypeObject> ((l) => new TypeObject(l.GetOutletType())));
			Negative =		new UnaryOperator("-",	   2,  Side.Right,	new UnOp<Int, Int>((l) => Value.Int(-l.Underlying)), 
																		new UnOp<Flt, Flt>((l) => Value.Float(-l.Underlying)),
																		new UnOp<Str, Str>((l) => new Str("olleh")));
			Not =			new UnaryOperator("!",	   2,  Side.Right,	new UnOp<Bln, Bln>((l) => Value.Bool(!l.Underlying)));
			Times =			new BinaryOperator("*",    3,  Side.Left,	new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying * r.Underlying)),
																		new BinOp<Flt, Flt, Flt>((l, r) => Value.Float(l.Underlying * r.Underlying)));
			Divide =		new BinaryOperator("/",    3,  Side.Left, 	new BinOp<Int, Int, Int>((l, r) => r.Underlying == 0 ? 
																			throw new RuntimeException("Divide By 0") : 
																			Value.Int(l.Underlying / r.Underlying)),
																		new BinOp<Flt, Flt, Flt>((l, r) => r.Underlying == 0 ?
																			throw new RuntimeException("Divide By 0") :
																			Value.Float(l.Underlying / r.Underlying)),
                                                                        new BinOp<TypeObject, TypeObject, TypeObject>((l, r) => new TypeObject(new UnionType(l.Encapsulated, r.Encapsulated))));
			Modulus =		new BinaryOperator("%",    3,  Side.Left, 	new BinOp<Int, Int, Int>((l, r) => r.Underlying == 0 ? 
																			throw new RuntimeException("Divide By 0") : 
																			Value.Int(l.Underlying % r.Underlying)));
			Plus =			new BinaryOperator("+",    4,  Side.Left, 	new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying + r.Underlying)),
																		new BinOp<Flt, Flt, Flt>((l, r) => Value.Float(l.Underlying + r.Underlying)),
																		new BinOp<Str, Obj, Str>((l, r) => new Str(l.Underlying + r.ToString())),
																		new BinOp<Obj, Str, Str>((l, r) => new Str(l.ToString() + r.Underlying)));
			Minus =			new BinaryOperator("-",    4,  Side.Left, 	new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying - r.Underlying)),
																		new BinOp<Flt, Flt, Flt>((l, r) => Value.Float(l.Underlying - r.Underlying)));
			LShift =		new BinaryOperator("<<",   5,  Side.Left, 	new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying << r.Underlying)));
			RShift =		new BinaryOperator(">>",   5,  Side.Left, 	new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying >> r.Underlying)));
			LT =			new BinaryOperator("<",    6,  Side.Left,	new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying < r.Underlying)));
			LTE =			new BinaryOperator("<=",   6,  Side.Left,  	new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying <= r.Underlying)));
			GT =			new BinaryOperator(">",    6,  Side.Left, 	new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying > r.Underlying)));
			GTE =			new BinaryOperator(">=",   6,  Side.Left, 	new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying >= r.Underlying)));
			As =			new BinaryOperator("as",   6,  Side.Left);
			Is =			new BinaryOperator("is",   6,  Side.Left);
			Isnt =			new BinaryOperator("isnt", 6,  Side.Left);
			BoolEquals =	new BinaryOperator("==",   7,  Side.Left, 	new BinOp<Obj, Obj, Bln>((l, r) => Value.Bool(l.Equals(r))));
			NotEqual =		new BinaryOperator("!=",   7,  Side.Left, 	new BinOp<Obj, Obj, Bln>((l, r) => Value.Bool(!l.Equals(r))));
			BitAnd =		new BinaryOperator("&",	   8,  Side.Left,	new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying & r.Underlying)));
			BitXor =		new BinaryOperator("^",    9,  Side.Left,  	new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying ^ r.Underlying)));
			BitOr =			new BinaryOperator("|",    10, Side.Left,  	new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying | r.Underlying)));
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

		private static Operands.Array Range(int start, int end, bool inc) {
			int i = inc ? 1 : 0;
			return new Operands.Array(Enumerable.Range(start, end - start + i).Select((x) => Value.Int(x)).ToArray());
		}
	}

	public class BinaryOperator : Operator {

		public readonly Overload<BinOp> Overloads;

		public BinaryOperator(string name, int p, Side a, params BinOp[] defaultoverloads) : base(name, p, a) {
			Overloads = new Overload<BinOp>(defaultoverloads);
		}

		// Right param is first due to shunting yard popping the right operand first
		public Expression Construct(Expression r, Expression l) {
			if (this == Is || this == Isnt) return new Is(l, r, this == Is);
			if (this == As) return new As(l, r);
            if (this == Dot && r is Literal<int> idx) return new TupleAccess(l, idx.Value);
			if (this == Dot && r is Variable member) return new MemberAccess(l, member);
            // TODO better error handling here
            if (this == Dot) throw new UnexpectedException("Only variable or tuple access allowed here");
			if (this == Lambda) return new Lambda(l, r);
			if (this == Equal) return new Assign(l, r);
            if (this == PlusEqual) return new Assign(l, new Binary(Plus.Name, l, r, Plus.Overloads));
            if (this == MinusEqual) return new Assign(l, new Binary(Minus.Name, l, r, Minus.Overloads));
            if (this == MultEqual) return new Assign(l, new Binary(Times.Name, l, r, Times.Overloads));
            if (this == DivEqual) return new Assign(l, new Binary(Divide.Name, l, r, Divide.Overloads));
            if (this == ModEqual) return new Assign(l, new Binary(Modulus.Name, l, r, Modulus.Overloads));
			if (this == LogicalAnd || this == LogicalOr) return new ShortCircuit(l, this, r);
			return new Binary(Name, l, r, Overloads);
		}
	}

	public class UnaryOperator : Operator {

		public readonly Overload<UnOp> Overloads;

		public UnaryOperator(string name, int p, Side a, params UnOp[] overloads) : base(name, p, a) {
			Overloads = new Overload<UnOp>(overloads);
		}
	}

}
