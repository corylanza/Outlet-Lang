using Outlet.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens
{
	public record Token(Func<string> ToStringFunc)
    {
		public override string ToString() => ToStringFunc();
	}

	public record Identifier(string Name) : Token(() => Name);

	// Literal values
	public abstract record TokenLiteral(Func<string> ToStringFunc) : Token(ToStringFunc);
	public record NullLiteral() : TokenLiteral(() => "null");
	public record IntLiteral(int Value) : TokenLiteral(() => Value.ToString());
	public record FloatLiteral(float Value) : TokenLiteral(() => Value.ToString());
	public record BoolLiteral(bool Value) : TokenLiteral(() => Value.ToString());
	public record StringLiteral(string Value) : TokenLiteral(() => Value.ToString());

	// Symbols
	public sealed record Keyword(string Name) : Symbol(Name);
	public sealed record DelimeterToken(string Name) : Symbol(Name);
	public sealed record OperatorToken(string Name, BinaryOperator? Binary, UnaryOperator? PreUnary, UnaryOperator? PostUnary) : Symbol(Name)
	{
		public bool HasBinaryOperation([NotNullWhen(true)] out BinaryOperator? binop) => (binop = Binary) is not null;
		public bool HasPreUnaryOperation([NotNullWhen(true)] out UnaryOperator? unop) => (unop = PreUnary) is not null;
		public bool HasPostUnaryOperation([NotNullWhen(true)] out UnaryOperator? unop) => (unop = PostUnary) is not null;
	}

	public abstract record Symbol : Token
	{
		private static readonly Dictionary<string, Symbol> AllTokens = new();
		public static Dictionary<string, Symbol> GetAllTokens => AllTokens;
		public static bool ContainsKey(string text) => AllTokens.ContainsKey(text);
		public static Token Get(string text) => AllTokens[text];

		// Only one instance of each symbol should ever be created
		// These are added to the static AllTokens dictionary, used for lookups
		protected Symbol(string Name) : base(() => Name)
        {
			AllTokens.Add(ToStringFunc(), this);
		}

		private static OperatorToken DefOperator(string symbol, UnaryOperator unop) => new(symbol, null, unop, null);
		private static OperatorToken DefOperator(string symbol, UnaryOperator preUnary, UnaryOperator postUnary) => new(symbol, null, preUnary, postUnary);
		private static OperatorToken DefOperator(string symbol, UnaryOperator preUnary, BinaryOperator binop) => new(symbol, binop, preUnary, null);
		private static OperatorToken DefOperator(string symbol, BinaryOperator binop) => new(symbol, binop, null, null);

		public static readonly DelimeterToken
			LeftParen = new("("),
			RightParen = new(")"),
			LeftBrace = new("["),
			RightBrace = new("]"),
			LeftCurly = new("{"),
			RightCurly = new("}"),
			Comma = new(","),
			Colon = new(":"),
			SemiC = new(";");

		public static readonly Keyword
			Extends = new("extends"),
			If = new("if"),
			Then = new("then"),
			Else = new("else"),
			For = new("for"),
			In = new("in"),
			While = new("while"),
			Return = new("return"),
			Static = new("static"),
			Class = new("class"),
			Operator = new("operator"),
			Using = new("using"),
			Var = new("var");

		public static readonly OperatorToken
			PostInc = DefOperator("++", new PreInc(), new PostInc()),
			PostDec = DefOperator("--", new PreDec(), new PostDec()),
			Plus = DefOperator("+", new UnaryPlus(), new Plus()),
			Minus = DefOperator("-", new Negative(), new Minus()),
			Divide = DefOperator("/", new Divide()),
			Times = DefOperator("*", new Times()),
			Modulus = DefOperator("%", new Modulus()),
			LT = DefOperator("<", new LT()),
			GT = DefOperator(">", new GT()),
			LShift = DefOperator("<<", new LShift()),
			RShift = DefOperator(">>", new RShift()),
			Complement = DefOperator("~", new Complement()),
			BitAnd = DefOperator("&", new UnaryAnd(), new BitAnd()),
			BitOr = DefOperator("|", new BitOr()),
			BitXor = DefOperator("^", new BitXor()),
			LogicalAnd = DefOperator("&&", new LogicalAndOp()),
			LogicalOr = DefOperator("||", new LogicalOrOp()),
			Not = DefOperator("!", new Not()),
			Equal = DefOperator("=", new Equal()),
			Lambda = DefOperator("=>", new LambdaOp()),
			PlusEqual = DefOperator("+=", new PlusEqual()),
			MinusEqual = DefOperator("-=", new MinusEqual()),
			DivEqual = DefOperator("/=", new DivEqual()),
			MultEqual = DefOperator("*=", new MultEqual()),
			ModEqual = DefOperator("%=", new ModEqual()),
			LTE = DefOperator("<=", new LTE()),
			GTE = DefOperator(">=", new GTE()),
			NotEqual = DefOperator("!=", new NotEqual()),
			BoolEquals = DefOperator("==", new BoolEquals()),
			DefineLookup = DefOperator("#", new DefineLookup()),
			As = DefOperator("as", new AsOp()),
			Is = DefOperator("is", new IsOp()),
			Isnt = DefOperator("isnt", new IsntOp()),
			Question = DefOperator("?", new TernaryQuestion()),
			Dot = DefOperator(".", new DotOp());
	}
}
