﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Lexing;
using Outlet.Parsing;
using Outlet.Tokens;
using Outlet.AST;
using Outlet.Checking;
using Outlet.Optimizing;
using Outlet.Interpreting;

namespace Outlet {
	public static class Program {
		public static void Main(string[] args) {
            if(args.Length == 0) REPL();
            if(args.Length == 1) RunFile(args[0]);
		}

        public static void RunFile(string path) {
            byte[] file = File.ReadAllBytes(path);
            byte[] bytes = file.Skip(3).ToArray();
            LinkedList<Token> lexout = Lexer.Scan(bytes);
            Declaration program = Parser.Parse(lexout);
			Checker c = new Checker();
			Interpreter eval = new Interpreter();
			program.Accept(c);
			program.Accept(eval);
			//Scope s = new Scope(null);
			//program.Resolve(s);
			//program.Execute(s);
			while (true) ;
        }

        public static void REPL() {
			//Scope s = new Scope();

			while (true) {
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("<enter an expression>");
				string input = "";
				while (input.Length == 0 || input.Count((c) => c == '{') != input.Count((c) => c == '}')) { 
					input += Console.ReadLine();
				}
                byte[] bytes = Encoding.ASCII.GetBytes(input);
                try {
                    LinkedList<Token> lexout = Lexer.Scan(bytes);
                    Declaration program = Parser.Parse(lexout);
					Checker c = new Checker();
					Interpreter eval = new Interpreter();
					program.Accept(c);
					Operand res = program.Accept(eval);
					if(res != null) Console.WriteLine("Expression returned " + res);
				} catch (OutletException ex) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                } 
            }
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
