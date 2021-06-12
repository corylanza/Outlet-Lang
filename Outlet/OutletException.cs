using Outlet.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet {
	public class OutletException : Exception {
		public List<string> Errors { get; set; }
		public int ErrorCount => Errors.Count;


		public OutletException() {
			Errors = new List<string>();
		}
		public OutletException(string s) : base(s)
		{
			Errors = new List<string>();
		}

        public OutletException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = new List<string>();
        }
    }

	public class LexerException : OutletException {
		public LexerException(string message) : base("Lexing Error: "+message) { }

        public LexerException()
        {
        }

        public LexerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class SyntaxException : OutletException
    {

        public int Line { get; private init; }
        public Range CharacterRange { get; private init; }

        public SyntaxException(string message, int line, Range characterRange) : base(message)
        {
            Line = line;
            CharacterRange = characterRange;
        }
    }

	public class ParserException : OutletException {

        public List<SyntaxException> SyntaxErrors { get; private init; }

        private static string ToErrorMessage(params SyntaxException[] syntaxErrors)
        {
            var sb = new StringBuilder($"Parsing failed, found {syntaxErrors.Length} syntax errors:\n");
            foreach(var error in syntaxErrors)
            {
                sb.Append(error.Message + "\n");
            }

            return sb.ToString();
        }

        public ParserException(params SyntaxException[] syntaxErrors) : base(ToErrorMessage(syntaxErrors))
        {
            SyntaxErrors = syntaxErrors.ToList();
        }
    }

	public class CheckerException : OutletException {
		public CheckerException(string s) : base("Checking error: "+s) { }

        public CheckerException()
        {
        }

        public CheckerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

	public class RuntimeException : OutletException {
		public RuntimeException(string message) : base(message) { }
		public RuntimeException(string message, Interpreting.Interpreter interpreter) : 
            base($"Runtime Error: {message}\n{string.Join("\n", interpreter.CallStack.Select(frame => frame.Call))}") { }

		public RuntimeException() { }

        public RuntimeException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UnexpectedException : OutletException
    {
        public UnexpectedException(string s) : base(s) { }

        public UnexpectedException()
        {
        }

        public UnexpectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
