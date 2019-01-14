using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Util;
using Outlet.Tokens;
//using State = Outlet.Util.State<Outlet.Lexing.CharType, Outlet.TokenType>;

namespace Outlet.Lexing {
    public static partial class Lexer {

		public static int LinePos = 0;
		public static int CharPos = 0;
		public static int ErrorCount = 0;
        
        static Lexer() {
            InitStates();
        }

		public static void Error(string message) {
			ErrorCount++;
			Program.ThrowException(message);
		}

        public static LinkedList<Token> Scan(byte[] charStream) {
			ErrorCount = 0;
			machine.Cur = start; 
			string buffer = "";
			LinkedList<Token> tokens = new LinkedList<Token>();
			for (int i = 0; i < charStream.Length; i++) {
                byte b = charStream[i];
				CharType c = CharClass.Get(b);
				if(machine.Peek(c)) {
					machine.NextState(c);
				} else if(machine.Cur.Accepting) {
					if(machine.Cur.Output != NoToken) {
						Token toadd = machine.Cur.Output(buffer);
						if(toadd != null) {
							if(tokens.Count > 1 && toadd is IntLiteral i2 && tokens.Last() == Operator.Dot) {
								tokens.RemoveLast();
								if(tokens.Last() is IntLiteral i1) {
									tokens.RemoveLast();
									tokens.AddLast(new FloatLiteral(i1.Value + "." + i2.Value, 0, 0));
								} else {
									tokens.AddLast(Operator.Dot);
									tokens.AddLast(i2);
								}
							} else tokens.AddLast(toadd);
						}
					}
					buffer = "";
					machine.Cur = start.Transition(c);
				} else {
					Error("illegal character");
				}
                if(machine.Cur.Keep) buffer += (char) b;
				CharPos++;
				if(c == CharType.NewLine) {
					LinePos++;
					CharPos = 0;
				}
            }
			if(!machine.Cur.Accepting && buffer.Length > 0) Error("illegal state");
			else if(buffer.Length > 0) {
				Token toadd = machine.Cur.Output(buffer);
				if(toadd != null) tokens.AddLast(toadd);
			} else if(machine.Cur == poststring) tokens.AddLast(new StringLiteral("", LinePos, CharPos));
			if(ErrorCount > 0) throw new LexerException(ErrorCount+" Lexing errors encountered");
            return tokens;
		}

}
}
