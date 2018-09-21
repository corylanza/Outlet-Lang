using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet {
	public class Keyword : IToken {

		public static readonly Keyword Var = new Keyword("var");
		public static readonly Keyword If = new Keyword("if");
		public static readonly Keyword Else = new Keyword("else");
		public static readonly Keyword For = new Keyword("for");
		public static readonly Keyword While = new Keyword("while");
		public static readonly Keyword Return = new Keyword("return");
		public string Name;
		private Keyword(string name) {
			Name = name;
		}
	}
}
