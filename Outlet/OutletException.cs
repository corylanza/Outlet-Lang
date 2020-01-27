using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet {
	public class OutletException : Exception {
		public OutletException() { }
		public OutletException(string s) : base(s) { }
	}

	public class LexerException : OutletException {
		public LexerException(string message) : base("Lexing Error: "+message) { }
	}

	public class ParserException : OutletException {
		public ParserException(string message) : base("Parsing Error: " + message) { }
	}

	public class CheckerException : OutletException {
		public CheckerException(string s) : base("Checking error: "+s) { }
	}

	public class RuntimeException : OutletException {
		public RuntimeException(string message) : base(message) { }
		public RuntimeException(string message, Interpreting.Interpreter interpreter) : 
            base("Runtime Error: " + message + "\n" + interpreter.CallStack.Select(frame => frame.Call + "\n").ToList().ToListString()) { }
	}
}
