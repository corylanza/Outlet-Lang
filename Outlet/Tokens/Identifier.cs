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

		public override bool Equals(object obj) => obj is Identifier id && id.Name == Name;
		public override int GetHashCode() => base.GetHashCode();
		public override string ToString() => Name;
	}
}
