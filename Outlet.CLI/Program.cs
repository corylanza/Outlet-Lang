using Outlet;
using Outlet.StandardLib;
using Outlet.ForeignFunctions;
using Outlet.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Outlet.CLI
{
	public static class Program
	{

		public static void Main(string[] args)
		{
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
			stdin: () => Console.ReadLine() ?? "",
			stdout: text => Console.WriteLine(text),
			stderr: stderror
		);

		//public static SystemInterface ConsoleInterface(List<string> inputLines, List<string> outputLines, StandardError stderror) => new SystemInterface(
		//	stdin: () => inputLines.Tak ?? "",
		//	stdout: text => Console.WriteLine(text),
		//	stderr: stderror
		//);

		public static void RunFile(string path)
		{
			if (!File.Exists(path)) ThrowException(new Exception("file does not exist"));
			else
			{
				byte[] file = File.ReadAllBytes(path);
				byte[] bytes = file.Skip(3).ToArray();
				new OutletProgramFile(bytes, ConsoleInterface(ThrowException));
				Console.ReadLine();
			}
		}

		public static void REPL()
		{
			var repl = new ReplOutletProgram(ConsoleInterface(ThrowException));
			while (true)
			{
				if(ConsoleClass.LexingMode)
                {
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("<enter outlet code>");
					string input = "";
					while (input.Length == 0 || input.Count((c) => c == '{') > input.Count((c) => c == '}'))
					{
						input += Console.ReadLine();
					}
					byte[] bytes = Encoding.ASCII.GetBytes(input);
					var output = repl.Tokenize(bytes);
					output.ToList().ForEach(lexeme => PrettyPrinter.PrettyPrint(lexeme.PrettyPrint().ToArray()));
				} else
                {
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("> ");
					string input = "";
					while (input.Length == 0 || input.Count((c) => c == '{') > input.Count((c) => c == '}'))
					{
						input += Console.ReadLine();
					}
					byte[] bytes = Encoding.ASCII.GetBytes(input);
					Console.WriteLine(repl.Run(bytes).ToString());
				}
			}
		}

		private static void ThrowException(Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(ex.Message);
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}
