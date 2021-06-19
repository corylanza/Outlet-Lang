using Outlet.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Outlet.Tokens
{
    public class OperatorToken : Symbol
    {
        public bool HasBinaryOperation([NotNullWhen(true)] out BinaryOperator? binop)
        {
            binop = Binary;
            return !(binop is null);
        }

        public bool HasPreUnaryOperation([NotNullWhen(true)] out UnaryOperator? unop)
        {
            unop = PreUnary; 
            return !(unop is null);
        }

        public bool HasPostUnaryOperation([NotNullWhen(true)] out UnaryOperator? unop)
        {
            unop = PostUnary;
            return !(unop is null);
        }

        private BinaryOperator? Binary { get; set; }
        private UnaryOperator? PreUnary { get; set; }
        private UnaryOperator? PostUnary { get; set; }

        private OperatorToken(string symbol, UnaryOperator? preUnary, UnaryOperator? postUnary, BinaryOperator? binop) : base(symbol) {
            PreUnary = preUnary;
            PostUnary = postUnary;
            Binary = binop;
        }

        private OperatorToken(string symbol, UnaryOperator unop) : this(symbol, unop, null, null) { }
        private OperatorToken(string symbol, UnaryOperator preUnary, UnaryOperator postUnary) : this(symbol, preUnary, postUnary, null) { }
        private OperatorToken(string symbol, UnaryOperator preUnary, BinaryOperator binop) : this(symbol, preUnary, null, binop) { }
        private OperatorToken(string symbol, BinaryOperator binop) : this(symbol, null, null, binop) { }

        public static readonly OperatorToken PostInc = new("++", new PreInc(), new PostInc());
        public static readonly OperatorToken PostDec = new("--", new PreDec(), new PostDec());
        public static readonly OperatorToken Plus = new("+", new UnaryPlus(), new Plus());
        public static readonly OperatorToken Minus = new("-", new Negative(), new Minus());
        public static readonly OperatorToken Divide = new("/", new Divide());
        public static readonly OperatorToken Times = new("*", new Times());
        public static readonly OperatorToken Modulus = new("%", new Modulus());
        public static readonly OperatorToken LT = new("<", new LT());
        public static readonly OperatorToken GT = new(">", new GT());
        public static readonly OperatorToken LShift = new("<<", new LShift());
        public static readonly OperatorToken RShift = new(">>", new RShift());
        public static readonly OperatorToken Complement = new("~", new Complement());
        public static readonly OperatorToken BitAnd = new("&", new UnaryAnd(), new BitAnd());
        public static readonly OperatorToken BitOr = new("|", new BitOr());
        public static readonly OperatorToken BitXor = new("^", new BitXor());
        public static readonly OperatorToken LogicalAnd = new("&&", new LogicalAndOp());
        public static readonly OperatorToken LogicalOr = new("||", new LogicalOrOp());
        public static readonly OperatorToken Not = new("!", new Not());
        public static readonly OperatorToken Equal = new("=", new Equal());
        public static readonly OperatorToken Lambda = new("=>", new LambdaOp());
        public static readonly OperatorToken PlusEqual = new("+=", new PlusEqual());
        public static readonly OperatorToken MinusEqual = new("-=", new MinusEqual());
        public static readonly OperatorToken DivEqual = new("/=", new DivEqual());
        public static readonly OperatorToken MultEqual = new("*=", new MultEqual());
        public static readonly OperatorToken ModEqual = new("%=", new ModEqual());
        public static readonly OperatorToken LTE = new("<=", new LTE());
        public static readonly OperatorToken GTE = new(">=", new GTE());
        public static readonly OperatorToken NotEqual = new("!=", new NotEqual());
        public static readonly OperatorToken BoolEquals = new("==", new BoolEquals());
        public static readonly OperatorToken DefineLookup = new("#", new DefineLookup());
        public static readonly OperatorToken As = new("as", new AsOp());
        public static readonly OperatorToken Is = new("is", new IsOp());
        public static readonly OperatorToken Isnt = new("isnt", new IsntOp());
        public static readonly OperatorToken Question = new("?", new TernaryQuestion());
        public static readonly OperatorToken Dot = new(".", new DotOp());
    }
}
