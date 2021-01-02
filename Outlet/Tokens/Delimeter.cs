using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {
	public class DelimeterToken : Symbol {

		private DelimeterToken(string name) : base(name) { }

		public static readonly DelimeterToken LeftParen = new DelimeterToken("(");
		public static readonly DelimeterToken RightParen = new DelimeterToken(")");
		public static readonly DelimeterToken LeftBrace = new DelimeterToken("[");
		public static readonly DelimeterToken RightBrace = new DelimeterToken("]");
		public static readonly DelimeterToken RightCurly = new DelimeterToken("}");
		public static readonly DelimeterToken LeftCurly = new DelimeterToken("{");
		public static readonly DelimeterToken Comma = new DelimeterToken(",");
		public static readonly DelimeterToken Colon = new DelimeterToken(":");
		public static readonly DelimeterToken SemiC = new DelimeterToken(";");
	}
}
