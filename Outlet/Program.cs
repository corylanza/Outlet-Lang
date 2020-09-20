using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Outlet.AST;
using Outlet.StandardLib;
using Outlet.Tokens;

namespace Outlet {
	public static class Program {

		public static void Main(string[] args) {
#if (DEBUG)
			REPL();
#else

			if (args.Length == 1) RunFile(args[0]);
			else {
				while (true) {
					Console.WriteLine("enter the name of the file:");
					string f = Console.ReadLine();
					RunFile(Directory.GetCurrentDirectory() + @"\Outlet\Test\" + f + ".txt");
				}
			}
#endif

		}

		public static SystemInterface ConsoleInterface(StandardError stderror) => new SystemInterface(
			stdin: () => Console.ReadLine(),
			stdout: text => Console.WriteLine(text),
			stderr: stderror
		);

		public static void RunFile(string path) {
			if (!File.Exists(path)) ThrowException(new Exception("file does not exist"));
			else {
				byte[] file = File.ReadAllBytes(path);
				byte[] bytes = file.Skip(3).ToArray();
				new OutletProgramFile(bytes, ConsoleInterface(ThrowException));
				Console.ReadLine();
			}
		}

		public static void REPL() {
			var repl = new ReplOutletProgram(ConsoleInterface(ThrowException));
			while (true) {
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("<enter an expression>");
				string input = "";
				while (input.Length == 0 || input.Count((c) => c == '{') > input.Count((c) => c == '}')) {
					input += Console.ReadLine();
				}
				byte[] bytes = Encoding.ASCII.GetBytes(input);
				Console.WriteLine(repl.Run(bytes).ToString());
			}
		}

		private static void ThrowException(Exception ex) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(ex.Message);
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static string ToListString<T>(this List<T> list) {
			string s = "";
			for (int i = 0; i < list.Count; i++) {
				s += list[i]?.ToString() ?? "";
				if (i != list.Count - 1) s += ", ";
			}
			return s;
		}

		public static bool SameLengthAndAll<T, U>(this IEnumerable<T> list, IEnumerable<U> other, Func<T, U, bool> predicate)
		{
			if (list.Count() != other.Count()) return false;
			for (int i = 0; i < list.Count(); i++)
			{
				if (!predicate(list.ElementAt(i), other.ElementAt(i))) return false;
			}
			return true;
		}

		public static IEnumerable<(F, S)> Zip<F, S>(this IEnumerable<F> first, IEnumerable<S> second)
        {
			if(first.Count() == second.Count())
            {
				return Enumerable.Range(0, first.Count()).Select(idx => (first.ElementAt(idx), second.ElementAt(idx)));
            }
			throw new Exception("can't zip collections of different lengths");
        }

		public static T Dequeue<T>(this LinkedList<T> ll)
		{
			T temp = ll.First();
			ll.RemoveFirst();
			return temp;
		}

		public static Literal ToLiteral(this TokenLiteral literal)
		{
			return literal switch
			{
				IntLiteral i => new Literal<int>(i.Value),
				FloatLiteral f => new Literal<float>(f.Value),
				BoolLiteral b => new Literal<bool>(b.Value),
				Tokens.StringLiteral s => new AST.StringLiteral(s.Value),
				NullLiteral _ => new NullExpr(),
				_ => throw new NotImplementedException()
			};
		}

		public static Variable ToVariable(this string s) => new Variable(s);
	}
}
