using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Util;
using State = Outlet.Util.State<Outlet.Lexing.CharType, Outlet.TokenType>;

namespace Outlet.Lexing {
	public static partial class Lexer {

        //static StateMachine<CharType, Tokenizer> machine2 = new StateMachine<CharType, Tokenizer>();
        static StateMachine<CharType, TokenType> machine = new StateMachine<CharType, TokenType>();
        static State error = machine.ErrorState(); // must be initialized first
        static State start = machine.AddStartState(); // starting / default state
        static State id = machine.AddState(true, TokenType.Identifier);
        static State number = machine.AddState(true, TokenType.OInt);
        static State dot = machine.AddState(false);
        static State sfloat = machine.AddState(true, TokenType.OFloat); // prevents double dec e.g. 34.2.3
        static State instring = machine.AddState(false, TokenType.None, instring); // after seeing a "
        //static State escaped = machine.AddState();


        static State comment = machine.AddState();
		

		static void InitStates() {
            start.SetTransition(CharType.Letter, id);
			start.SetTransition(CharType.Number, number);
            start.SetTransition(CharType.Whitespace, start);
			id.SetTransition(CharType.Number, id);
            id.SetTransition(CharType.Letter, id);
            id.SetTransition(CharType.Whitespace, start);
            number.SetTransition(CharType.Number, number);
            number.SetTransition(CharType.Letter, error);
            number.SetTransition(CharType.Whitespace, start);
            number.SetTransition(CharType.Dot, dot);
            dot.SetTransition(CharType.Number, sfloat);
            sfloat.SetTransition(CharType.Whitespace, start);
            start.SetTransition(CharType.Quo, instring);
            instring.SetTransition(CharType.Quo, start);
        }
	}
}
