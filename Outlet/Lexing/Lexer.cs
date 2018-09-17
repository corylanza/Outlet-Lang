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
        
        static Lexer() {
            InitStates();
        }


        public static Queue<Token> Scan(byte[] charStream) {
            Queue<Token> tokens = new Queue<Token>();
            for(int i = 0; i < charStream.Length; i++) {
                byte b = charStream[i];
               
                CharType c = CharClass.Get(b);
                if(machine.Peek(c)) {
                    machine.NextState(c);
                } else if(machine.Cur.Accepting) { 
                    if(machine.Cur.Output != NoToken) tokens.Enqueue(machine.Cur.Output(buffer));
                    buffer = "";
                    machine.Cur = start.Transition(c);
                } else throw new Exception("illegal");
                if(machine.Cur.Keep) buffer += (char) b;
            }
            if(!machine.Cur.Accepting && buffer.Length > 0) throw new Exception("illegal");
            else if(buffer.Length > 0) tokens.Enqueue(machine.Cur.Output(buffer));
            return tokens;
		}

}
}
