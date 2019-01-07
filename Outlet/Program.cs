using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Lexing;
using Outlet.Tokens;
using Outlet.Parsing;
using Outlet.AST;
using Outlet.Checking;
using Outlet.Operands;
using Outlet.Interpreting;

namespace Outlet {
	public static class Program {

		public static void Main(string[] args) {
            if(args.Length == 0) REPL();
			if(args.Length == 1 && args[0] == "run") {
				while(true) {
					Console.WriteLine("enter the name of the file:");
					string f = Console.ReadLine();
					RunFile(Directory.GetCurrentDirectory()+@"\Outlet\Test\" + f + ".txt");
				}
			} else if(args.Length == 1) RunFile(args[0]);
		}

        public static void RunFile(string path) {
            byte[] file = File.ReadAllBytes(path);
            byte[] bytes = file.Skip(3).ToArray();
			Run(bytes);
        }

        public static void REPL() {
			while (true) {
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("<enter an expression>");
				string input = "";
				while (input.Length == 0 || input.Count((c) => c == '{') != input.Count((c) => c == '}')) { 
					input += Console.ReadLine();
				}
                byte[] bytes = Encoding.ASCII.GetBytes(input);
				Run(bytes);
            }
        }

		public static void Run(byte[] bytes) {
			try {
				LinkedList<Token> lexout = Lexer.Scan(bytes);
				IASTNode program = Parser.Parse(lexout);
				Checker.Check(program);
				Operand res = Interpreter.Interpret(program);
				if(res != null) Console.WriteLine("Expression returned " + res);
			} catch(OutletException ex) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		public static void ThrowException(string message) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static List<(T, V)> TupleZip<T, V>(this List<T> list, List<V> other) {
			if(other.Count() == list.Count()) {
				List<(T, V)> output = new List<(T, V)>();
				for(int i = 0; i < list.Count(); i++) {
					output.Add((list[i], other[i]));
				}
				return output;
			}
			throw new Exception("lists of differing length");
		}

		public static string ToListString<T>(this List<T> list) {
			string s = "";
			for(int i = 0; i < list.Count; i++) {
				s += list[i].ToString();
				if (i != list.Count - 1) s += ", ";
			}
			return s;
		}

		public static T MinElement<T>(this IEnumerable<T> list, Func<T, int> f) {
			int min = int.MaxValue;
			T res = default;
			foreach(T t in list) {
				int cur = f(t);
				if(cur <= min) {
					min = cur;
					res = t;
				}
			}
			return res;
		}
	}
}
