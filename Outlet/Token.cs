using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Expressions;

namespace Outlet {

	public enum TokenType {
        None = 0,
        // One Character
        LeftParen, RightParen, LeftBrace, RightBrace,
        LeftCurly, RightCurly, Colon,
        Comma, Dot, SemiC,

        // Operators that can have = appended
        Plus, Minus, Divide, Multiply, LT, GT, Not, Equal,
        PlusEqual, MinusEqual, MultEqual, DividedEqual, LTE, GTE, NotEqual, BoolEquals,

        // multichar
        Identifier, OString, OInt, OFloat,

        // keywords
        If, Else, Null, For, While, Return, True, False,
        // functions, increment ops, bitshifts, and logical ops not represented yet


        EOF
	}

	public interface IToken {
        /*
        public static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>() {
            {"if", TokenType.If },
            {"else", TokenType.Else },
            {"null", TokenType.Null },
            {"for", TokenType.For },
            {"while", TokenType.While },
            {"return", TokenType.Return },
            {"true", TokenType.True },
            {"false", TokenType.False }
        };

        public static readonly Dictionary<string, TokenType> Delimeters = new Dictionary<string, TokenType>() {
            {"(", TokenType.LeftParen },
            {")", TokenType.RightParen },
            {"[", TokenType.LeftBrace },
            {"]", TokenType.RightBrace },
            {"{", TokenType.RightCurly },
            {"}", TokenType.LeftCurly },
            {",", TokenType.Comma },
            {":", TokenType.Colon },
            {";", TokenType.SemiC },
            {".", TokenType.Dot }
        };

        public static readonly Dictionary<string, TokenType> PreEquals = new Dictionary<string, TokenType>() {
            {"+", TokenType.Plus },
            {"-", TokenType.Minus },
            {"/", TokenType.Divide },
            {"*", TokenType.Multiply },
            {"<", TokenType.LT },
            {">", TokenType.GT },
            {"!", TokenType.Not },
            {"=", TokenType.Equal },
            {"+=", TokenType.PlusEqual },
            {"-=", TokenType.MinusEqual },
            {"/=", TokenType.DividedEqual },
            {"*=", TokenType.MultEqual },
            {"<=", TokenType.LTE },
            {">=", TokenType.GTE },
            {"!=", TokenType.NotEqual },
            {"==", TokenType.BoolEquals }
        };*/
        /*
        public object Value;
        public TokenType Type;
        public IToken(object text, TokenType type) {
            Value = text;
            Type = type;
        }


        
        public override string ToString() => Value + " type: " + Type.ToString();*/
    }

    public class Identifier : IToken {
        public string Name;
        public Identifier(string name){Name = name;}
    }

    public class Keyword : IToken {

        public static readonly Keyword If = new Keyword("if");
        public static readonly Keyword Else = new Keyword("else");
        public static readonly Keyword For = new Keyword("for");
        public static readonly Keyword While = new Keyword("while");
        public static readonly Keyword Return = new Keyword("return");
        public string Name;
        private Keyword(string name) {
            Name = name;
        }

        public static IToken Get(string text) => Keywords[text];
        public static bool ContainsKey(string text) => Keywords.ContainsKey(text);

        public static readonly Dictionary<string, IToken> Keywords = new Dictionary<string, IToken>() {
            {"if", If },
            {"else", Else },
            //{"null", TokenType.Null },
            {"for", For },
            {"while", While },
            {"return", Return },
            //{"true", new Literal(true) },
            //{"false", new Literal(false) }
        };
    }

    public enum Side { Left, Right }

    public class Operator : IToken {
        public static Operator Plus = new Operator(4, Side.Left, (l, r) => l.Value+r.Value);

        public int Precedence;
        public Side Asssoc;
        private Func<Operand, Operand, Operand> Function;
        private Operator(int precedence, Side associativity, Func<Operand, Operand, Operand> func) {
            Precedence = precedence;
            Asssoc = associativity;
            Function = func;
        }
        public Operand PerformOp(Operand l, Operand r) => Function(l, r);
        public static Operator Get(string text) => Plus;
    }

    public class Literal : Operand, IToken {
        public Literal(int value) {
            Value = value;
        }

        public Literal(string value) {
            Value = value;
        }

        public Literal(float value) {
            Value = value;
        }

        public Literal(bool value) {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
