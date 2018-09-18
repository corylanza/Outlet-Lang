using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet {
	public class Delimeter : IToken {

		public static readonly Delimeter LeftParen = new Delimeter();
		public static readonly Delimeter RightParen = new Delimeter();
		public static readonly Delimeter LeftBrace = new Delimeter();
		public static readonly Delimeter RightBrace = new Delimeter();
		public static readonly Delimeter RightCurly = new Delimeter();
		public static readonly Delimeter LeftCurly = new Delimeter();
		public static readonly Delimeter Comma = new Delimeter();
		public static readonly Delimeter Colon = new Delimeter();
		public static readonly Delimeter SemiC = new Delimeter();

		private Delimeter() { }
	}
}
