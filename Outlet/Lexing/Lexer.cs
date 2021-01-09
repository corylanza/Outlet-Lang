using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Util;
using Outlet.Tokens;
using Outlet.StandardLib;
using Outlet.Lexer;

namespace Outlet.Lexing {
    public partial class Lexer : ILexer {

		private int LinePos = 1;
		private int CharPos = 1;
		private int ErrorCount = 0;
        
        public Lexer() {
            InitStates();
        }

		private void Error(string message, StandardError errorHandler) {
			ErrorCount++;
			errorHandler(new LexerException(message));
		}

        public LinkedList<Lexeme> Scan(byte[] charStream, StandardError errorHandler) {
			LinePos = 1;
			CharPos = 1;
			ErrorCount = 0;
			machine.Cur = start; 
			string buffer = "";
			LinkedList<Lexeme> lexemes = new LinkedList<Lexeme>();
			for (int i = 0; i < charStream.Length; i++) {
                byte b = charStream[i];
				CharType c = CharClass.Get(b);
				if(machine.Peek(c)) {
					machine.NextState(c);
				} else if(machine.Cur.Accepting) {
					if(machine.Cur.Output != NoToken) {
						Token? toadd = machine.Cur.Output is Tokenizer t ? t(buffer) : null;
						if(toadd != null) {
							if(lexemes.Count > 1 && toadd is IntLiteral i2 && lexemes.Last() is Lexeme dotLexeme && dotLexeme.InnerToken == OperatorToken.Dot) {
								lexemes.RemoveLast();
								if(lexemes.Last() is Lexeme l1 && l1.InnerToken is IntLiteral i1) {
									// if there was an int . int then create a float token instead use the first ints position as the new floats position
									lexemes.RemoveLast();
									lexemes.AddLast(new Lexeme(new FloatLiteral(i1.Value + "." + i2.Value), l1.Line, l1.Character));
								} else {
									// if we don't find an int . int pattern then we must readd the . and add an int lexeme
									lexemes.AddLast(dotLexeme);
									lexemes.AddLast(new Lexeme(i2, LinePos, CharPos));
								}
							} else lexemes.AddLast(new Lexeme(toadd, LinePos, CharPos));
						}
					}
					buffer = "";
					machine.Cur = start.Transition(c);
				} else {
					Error("illegal character", errorHandler);
				}
                if(machine.Cur.Keep) buffer += (char) b;
				CharPos++;
				if(c == CharType.NewLine) {
					LinePos++;
					CharPos = 0;
				}
            }
			if(!machine.Cur.Accepting && buffer.Length > 0) Error("illegal state", errorHandler);
			else if(buffer.Length > 0) {
				Token? toadd = machine.Cur.Output is Tokenizer t ? t (buffer) : null;
				if(toadd != null) lexemes.AddLast(new Lexeme(toadd, LinePos, CharPos));
			} else if(machine.Cur == poststring) lexemes.AddLast(new Lexeme(new StringLiteral(""), LinePos, CharPos));
			if(ErrorCount > 0) throw new LexerException(ErrorCount+" Lexing errors encountered");
            return lexemes;
		}

}
}
