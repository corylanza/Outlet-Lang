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
        Comma, Dot, Plus, Minus, SemiC, Divide, Multiply,

        // One or two chars
        Not, NotEqual,
        Equal, BoolEquals,
        GT, GTE, LT, LTE,

        // multichar
        Identifier, OString, OInt, OFloat,

        // keywords
        If, Else, Null, For, While, Return, True, False,
        // function keywords and assignment ops such as += or ++ not represented yet


        EOF
	}

    public delegate Token Tokenizer(string input);

	public class Token {
        public string Text;
        public TokenType Type;
        public Token(string text, TokenType type) {
            Text = text;
            Type = type;
        }
	}
}
