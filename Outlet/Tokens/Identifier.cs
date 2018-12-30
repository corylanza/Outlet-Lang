using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {
	public class Identifier : Token {

		public readonly string Name;
		public int Line, Pos;

		public Identifier(string name, int linenumber, int posinline) {
			Name = name;
			Line = linenumber;
			Pos = posinline;
		}

		public override string ToString() => Name;
	}
}
