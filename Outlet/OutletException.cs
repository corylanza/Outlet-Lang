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

	public class ParserException : OutletException {
		public ParserException(string message) : base("Parsing Error: " + message) { }

        public ParserException()
        {
        }

        public ParserException(string message, Exception innerException) : base(message, innerException)
        {
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
            base("Runtime Error: " + message + "\n" + interpreter.CallStack.Select(frame => frame.Call + "\n").ToList().ToListString()) { }

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
