using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet {
	public class Program {
		public static void Main(string[] args) { 
             //byte[] file = File.ReadAllBytes("C:\\");
            string input = "there 34.3 and h1";
            byte[] bytes = Encoding.ASCII.GetBytes(input);
			List<Token> lexout = Lexing.Lexer.Scan(bytes);
            foreach(Token t in lexout) {
                Console.WriteLine(t.Text + " type: " + Enum.GetName(t.Type.GetType(), t.Type));
            }
            while(true) ;
		}
	}
}
