using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet {
	public class Keyword : IToken {

		public static readonly Keyword True = new Keyword("true");
		public static readonly Keyword False = new Keyword("false");
		public static readonly Keyword Null = new Keyword("null");
		public static readonly Keyword Var = new Keyword("var");
		public static readonly Keyword If = new Keyword("if");
		public static readonly Keyword Then = new Keyword("then");
		public static readonly Keyword Else = new Keyword("else");
		public static readonly Keyword For = new Keyword("for");
		public static readonly Keyword In = new Keyword("in");
		public static readonly Keyword While = new Keyword("while");
		public static readonly Keyword Return = new Keyword("return");
		public static readonly Keyword Static = new Keyword("static");
		//public static readonly Keyword Void = new Keyword("void");
		//public static readonly Keyword Func = new Keyword("func");
		public static readonly Keyword Class = new Keyword("class");
		public static readonly Keyword Operator = new Keyword("operator");
		public readonly string Name;
		private Keyword(string name) {
			Name = name;
		}
	}
}
