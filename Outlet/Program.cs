using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Lexing;
using Outlet.Parsing;
using Outlet.AST;

namespace Outlet {
	public static class Program {
		public static void Main(string[] args) {
            if(args.Length == 0) REPL();
            if(args.Length == 1) RunFile(args[0]);
		}

        public static void RunFile(string path) {
            Scope s = new Scope();
            byte[] file = File.ReadAllBytes(path);// Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/Test/file2.txt")
            byte[] bytes = file.Skip(3).ToArray();
            Queue<IToken> lexout = Lexer.Scan(bytes);
            Statement program = Parser.Parse(s, lexout);
            program.Execute();
        }

        public static void REPL() {
            Scope s = new Scope(true); // used by repl to keep definitions
            while(true) {
                Console.WriteLine("<enter an expression>");
                string input = Console.ReadLine();
                //byte[] file = File.ReadAllBytes(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/Test/file2.txt");
                byte[] bytes = Encoding.ASCII.GetBytes(input);// file.Skip(3).ToArray());
                Queue<IToken> lexout = Lexer.Scan(bytes);
                Statement program = Parser.Parse(s, lexout);
                //Console.WriteLine("Parsed: " + program.ToString());
                if(program is Expression e) Console.WriteLine("Expression returned " + e.Eval());
                else {
                    program.Execute();
                }
                s.Lines.Clear();
            }
        }

    }
}
