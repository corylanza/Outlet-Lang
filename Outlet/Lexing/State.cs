using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Util;
using State = Outlet.Util.State<Outlet.Lexing.CharType, Outlet.Lexing.Lexer.Tokenizer>;

namespace Outlet.Lexing {
    public static partial class Lexer {

        static StateMachine<CharType, Tokenizer> machine = new StateMachine<CharType, Tokenizer>();
        static State start = machine.AddStartState(); // starting / default state
        static State id = machine.AddState(true, true, TokenizeID);
        // ints and floats
        static State number = machine.AddState(true, true, TokenizeInt);
        static State dot = machine.AddState(false, true); // after seeing a .
        static State sfloat = machine.AddState(true, true, TokenizeFloat); // prevents double dec e.g. 34.2.3
        // strings
        static State prestring = machine.AddState(false, false); // after seeing a "
        static State instring = machine.AddState(false, true);
        static State poststring = machine.AddState(true, false, TokenizeString);

        //static State escaped = machine.AddState();


        static State comment = machine.AddState();


        static void InitStates() {
            start.SetTransition(CharType.Letter, id);
            start.SetTransition(CharType.Number, number);
            start.SetTransition(CharType.Whitespace, start);
            id.SetTransition(CharType.Number, id);
            id.SetTransition(CharType.Letter, id);
            //id.SetTransition(CharType.Whitespace, start);
            number.SetTransition(CharType.Number, number);
            //SetTransition(CharType.Whitespace, start);
            number.SetTransition(CharType.Dot, dot);
            dot.SetTransition(CharType.Number, sfloat);
            //sfloat.SetTransition(CharType.Whitespace, start);
            start.SetTransition(CharType.Quo, prestring);
            prestring.SetTransition(CharType.Quo, poststring);
            prestring.SetTransition(CharType.Letter, instring);
            prestring.SetTransition(CharType.Number, instring);
            prestring.SetTransition(CharType.Whitespace, instring);
            instring.SetTransition(CharType.Quo, poststring);
            instring.SetTransition(CharType.Letter, instring);
            instring.SetTransition(CharType.Number, instring);
            instring.SetTransition(CharType.Whitespace, instring);
        }

        public delegate Token Tokenizer(string text);

        private static Token TokenizeID(string text) {
            if(Token.Keywords.ContainsKey(text)) return new Token(text, Token.Keywords[text]);
            else return new Token(text, TokenType.Identifier);
        }

        private static Token TokenizeString(string text) => new Token(text, TokenType.OString);
        private static Token TokenizeInt(string text) => new Token(text, TokenType.OInt);
        private static Token TokenizeFloat(string text) => new Token(text, TokenType.OFloat);

    }
}