﻿using System;
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

        // operators
        static State SingleCharOp = machine.AddState(true, true, TokenizeSingleOp);
        static State plus = machine.AddState(true, true, TokenizePreEquals);
        static State minus = machine.AddState(true, true, TokenizePreEquals);
        static State forwardslash = machine.AddState(true, true, TokenizePreEquals);
        static State lt = machine.AddState(true, true, TokenizePreEquals);
        static State gt = machine.AddState(true, true, TokenizePreEquals);
        static State preequal = machine.AddState(true, true, TokenizePreEquals);
        static State withequal = machine.AddState(true, true, TokenizePreEquals);


        //static State comment = machine.AddState(true, true, NoToken);


        static void InitStates() {
            start.SetTransition(CharType.Letter, id);
            start.SetTransition(CharType.Number, number);
            start.SetTransition(CharType.Op1, SingleCharOp);
            start.SetTransition(CharType.Dot, SingleCharOp);
            start.SetTransition(CharType.Whitespace, start);
            start.SetTransition(CharType.NewLine, start);
            id.SetTransition(CharType.Number, id);
            id.SetTransition(CharType.Letter, id);
            // comments
            start.SetTransition(CharType.ForwardSlash, forwardslash);
            //forwardslash.SetTransition(CharType.ForwardSlash, comment);
            //comment.setDefault
            //comment.SetTransition(CharType.NewLine, start);
            // Ints and floats
            number.SetTransition(CharType.Number, number);
            number.SetTransition(CharType.Dot, dot);
            dot.SetTransition(CharType.Number, sfloat);
            // Strings
            start.SetTransition(CharType.Quo, prestring);
            //prestring.SetDefault
            //instring.SetDefault
            prestring.SetTransition(CharType.Quo, poststring);
            prestring.SetTransition(CharType.Letter, instring);
            prestring.SetTransition(CharType.Number, instring);
            prestring.SetTransition(CharType.Whitespace, instring);
            prestring.SetTransition(CharType.NewLine, instring);
            instring.SetTransition(CharType.Quo, poststring);
            instring.SetTransition(CharType.Letter, instring);
            instring.SetTransition(CharType.Number, instring);
            instring.SetTransition(CharType.Whitespace, instring);
            instring.SetTransition(CharType.NewLine, instring);
            // Operators
            start.SetTransition(CharType.Plus, plus);
            start.SetTransition(CharType.Minus, minus);
            start.SetTransition(CharType.LT, lt);
            start.SetTransition(CharType.GT, gt);
            start.SetTransition(CharType.OpEq, preequal);
            start.SetTransition(CharType.Equals, preequal);
            preequal.SetTransition(CharType.Equals, withequal);
            plus.SetTransition(CharType.Equals, withequal);
            minus.SetTransition(CharType.Equals, withequal);
            lt.SetTransition(CharType.Equals, withequal);
            gt.SetTransition(CharType.Equals, withequal);
        }

        public delegate Token Tokenizer(string text);

        private static Token TokenizeID(string text) {
            if(Token.Keywords.ContainsKey(text)) return new Token(text, Token.Keywords[text]);
            else return new Token(text, TokenType.Identifier);
        }
        private static Token TokenizeSingleOp(string text) => new Token(text, Token.Delimeters[text]);
        private static Token TokenizeString(string text) => new Token(text, TokenType.OString);
        private static Token TokenizeInt(string text) => new Token(text, TokenType.OInt);
        private static Token TokenizeFloat(string text) => new Token(text, TokenType.OFloat);
        private static Token TokenizePreEquals(string text) => new Token(text, Token.PreEquals[text]);
        private static Token NoToken(string text) => null;
    }
}