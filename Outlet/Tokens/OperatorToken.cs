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

        public static readonly OperatorToken PostInc = new OperatorToken("++", Operator.PreInc, Operator.PostInc);
        public static readonly OperatorToken PostDec = new OperatorToken("--", Operator.PreDec, Operator.PostDec);
        public static readonly OperatorToken Plus = new OperatorToken("+", Operator.UnaryPlus, Operator.Plus);
        public static readonly OperatorToken Minus = new OperatorToken("-", Operator.Negative, Operator.Minus);
        public static readonly OperatorToken Divide = new OperatorToken("/", Operator.Divide);
        public static readonly OperatorToken Times = new OperatorToken("*", Operator.Times);
        public static readonly OperatorToken Modulus = new OperatorToken("%", Operator.Modulus);
        public static readonly OperatorToken LT = new OperatorToken("<", Operator.LT);
        public static readonly OperatorToken GT = new OperatorToken(">", Operator.GT);
        public static readonly OperatorToken LShift = new OperatorToken("<<", Operator.LShift);
        public static readonly OperatorToken RShift = new OperatorToken(">>", Operator.RShift);
        public static readonly OperatorToken Complement = new OperatorToken("~", Operator.Complement);
        public static readonly OperatorToken BitAnd = new OperatorToken("&", Operator.UnaryAnd, Operator.BitAnd);
        public static readonly OperatorToken BitOr = new OperatorToken("|", Operator.BitOr);
        public static readonly OperatorToken BitXor = new OperatorToken("^", Operator.BitXor);
        public static readonly OperatorToken LogicalAnd = new OperatorToken("&&", Operator.LogicalAnd);
        public static readonly OperatorToken LogicalOr = new OperatorToken("||", Operator.LogicalOr);
        public static readonly OperatorToken Not = new OperatorToken("!", Operator.Not);
        public static readonly OperatorToken Equal = new OperatorToken("=", Operator.Equal);
        public static readonly OperatorToken Lambda = new OperatorToken("=>", Operator.Lambda);
        public static readonly OperatorToken PlusEqual = new OperatorToken("+=", Operator.PlusEqual);
        public static readonly OperatorToken MinusEqual = new OperatorToken("-=", Operator.MinusEqual);
        public static readonly OperatorToken DivEqual = new OperatorToken("/=", Operator.DivEqual);
        public static readonly OperatorToken MultEqual = new OperatorToken("*=", Operator.MultEqual);
        public static readonly OperatorToken ModEqual = new OperatorToken("%=", Operator.ModEqual);
        public static readonly OperatorToken LTE = new OperatorToken("<=", Operator.LTE);
        public static readonly OperatorToken GTE = new OperatorToken(">=", Operator.GTE);
        public static readonly OperatorToken NotEqual = new OperatorToken("!=", Operator.NotEqual);
        public static readonly OperatorToken BoolEquals = new OperatorToken("==", Operator.BoolEquals);
        public static readonly OperatorToken DefineLookup = new OperatorToken("#", Operator.DefineLookup);
        public static readonly OperatorToken As = new OperatorToken("as", Operator.As);
        public static readonly OperatorToken Is = new OperatorToken("is", Operator.Is);
        public static readonly OperatorToken Isnt = new OperatorToken("isnt", Operator.Isnt);
        public static readonly OperatorToken Question = new OperatorToken("?", Operator.Question);
        public static readonly OperatorToken Dot = new OperatorToken(".", Operator.Dot);
    }
}
