using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Util;
using Outlet.AST;
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
        static State plus = machine.AddState(true, true, TokenizeOp);
        static State minus = machine.AddState(true, true, TokenizeOp);
		static State and = machine.AddState(true, true, TokenizeOp);
		static State or = machine.AddState(true, true, TokenizeOp);
		static State forwardslash = machine.AddState(true, true, TokenizeOp);
        static State lt = machine.AddState(true, true, TokenizeOp);
        static State gt = machine.AddState(true, true, TokenizeOp);
		static State equal = machine.AddState(true, true, TokenizeOp);
        static State preequal = machine.AddState(true, true, TokenizeOp); // operators that a = can be added to
        static State finalop = machine.AddState(true, true, TokenizeOp); //no more chars can be added to this op


        static State comment = machine.AddState(false, false, NoToken);
		static State commentesc = machine.AddState(false, false, NoToken);
		static State commentend = machine.AddState(true, false, NoToken);


		static void InitStates() {
            start.SetTransition(CharType.Letter, id);
            start.SetTransition(CharType.Number, number);
            start.SetTransition(CharType.OneChar, finalop);
            start.SetTransition(CharType.Dot, finalop);
            start.SetTransition(CharType.Whitespace, start);
            start.SetTransition(CharType.NewLine, start);
            id.SetTransition(CharType.Number, id);
            id.SetTransition(CharType.Letter, id);
            // comments
            start.SetTransition(CharType.ForwardSlash, forwardslash);
            forwardslash.SetTransition(CharType.Asterisk, comment);
			comment.SetDefualt(comment);
			comment.SetTransition(CharType.Asterisk, commentesc);
			commentesc.SetTransition(CharType.ForwardSlash, commentend);
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
			start.SetTransition(CharType.Asterisk, preequal);
			start.SetTransition(CharType.And, and);
			start.SetTransition(CharType.Or, or);
            start.SetTransition(CharType.LT, lt);
            start.SetTransition(CharType.GT, gt);
            start.SetTransition(CharType.OpEq, preequal);
			start.SetTransition(CharType.Equals, equal);
			start.SetTransition(CharType.Question, finalop);
			equal.SetTransition(CharType.GT, finalop);
			equal.SetTransition(CharType.Equals, finalop);
			preequal.SetTransition(CharType.Equals, finalop);
            plus.SetTransition(CharType.Equals, finalop);
			plus.SetTransition(CharType.Plus, finalop);
			minus.SetTransition(CharType.Equals, finalop);
			minus.SetTransition(CharType.Minus, finalop);
			and.SetTransition(CharType.And, finalop);
			and.SetTransition(CharType.Equals, finalop);
			or.SetTransition(CharType.Or, finalop);
			or.SetTransition(CharType.Equals, finalop);
            lt.SetTransition(CharType.Equals, finalop);
			lt.SetTransition(CharType.LT, preequal);
            gt.SetTransition(CharType.Equals, finalop);
			gt.SetTransition(CharType.GT, preequal);
        }

        public delegate IToken Tokenizer(string text);

        private static IToken TokenizeID(string text) {
            if(Token.ContainsKey(text)) return Token.Get(text);
            else return new Identifier(text);
        }
        private static IToken TokenizeOp(string text) => Token.Get(text);
        private static IToken TokenizeString(string text) => new Literal(text);
        private static IToken TokenizeInt(string text) => new Literal(int.Parse(text));
        private static IToken TokenizeFloat(string text) => new Literal(float.Parse(text));
        private static IToken NoToken(string text) => null;
    }
}