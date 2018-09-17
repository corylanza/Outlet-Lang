using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	public class Token {

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
        };

        public object Value;
        public TokenType Type;
        public Token(object text, TokenType type) {
            Value = text;
            Type = type;
        }


        
        public override string ToString() => Value + " type: " + Type.ToString();
    }
}
