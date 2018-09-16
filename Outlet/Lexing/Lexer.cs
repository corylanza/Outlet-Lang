using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Util;
using State = Outlet.Util.State<Outlet.Lexing.CharType, Outlet.TokenType>;

namespace Outlet.Lexing {
	public static partial class Lexer {

        private static string buffer = "";
        private static List<Token> tokens = new List<Token>();
		static Lexer() {
            InitStates();
		}
		
		
		public static List<Token> Scan(byte[] charStream) {
            for(int i = 0; i < charStream.Length; i++) {
                byte b = charStream[i];
                
                if(!machine.Peek(CharClass.Get(b)) || i == charStream.Length-1) {
                    tokens.Add(new Token(buffer, machine.Cur.Output));
                    buffer = "";
                } else buffer += (char) b;
                machine.NextState(CharClass.Get(b));
                if(machine.Cur == error) throw new Exception("illegal char sequence");

            } return tokens;
		}

	}
}
