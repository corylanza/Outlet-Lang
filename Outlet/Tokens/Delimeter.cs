using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {
	public class Delimeter : Token {

		public static readonly Delimeter FuncParen = new Delimeter("(");
		public static readonly Delimeter LeftParen = new Delimeter("(");
		public static readonly Delimeter RightParen = new Delimeter(")");
		public static readonly Delimeter IndexBrace = new Delimeter("[");
		public static readonly Delimeter LeftBrace = new Delimeter("[");
		public static readonly Delimeter RightBrace = new Delimeter("]");
		public static readonly Delimeter RightCurly = new Delimeter("}");
		public static readonly Delimeter LeftCurly = new Delimeter("{");
		public static readonly Delimeter Comma = new Delimeter(",");
		public static readonly Delimeter Colon = new Delimeter(":");
		public static readonly Delimeter SemiC = new Delimeter(";");

		public string Name;

		private Delimeter(string name) {
			Name = name;
		}

		public override string ToString() => Name;
	}
}
