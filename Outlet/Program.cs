using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Lexing;
using Outlet.Parsing;
using Outlet.Expressions;

namespace Outlet {
	public static class Program {
		public static void Main(string[] args) { 
            byte[] file = File.ReadAllBytes(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName+"/Test/file.txt");
            //string input = "  for+(  nu)mber 3\"some text\"and //\n+= 34.1";
            //byte[] bytes = Encoding.ASCII.GetBytes(input);
			Queue<Token> lexout = Lexer.Scan(file.Skip(3).ToArray());
            Expression expr = Parser.Parse(lexout);
            object returnValue = expr.Eval();
            
            foreach(Token t in lexout) {
                Console.WriteLine(t.ToString());
            }
            while(true) ;
		}

        public static string ToString(this TokenType token) => Enum.GetName(token.GetType(), token);

    }
}
