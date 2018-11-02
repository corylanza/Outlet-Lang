using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {
	public class Identifier : Token {

		public readonly string Name;
		public Identifier(string name) { Name = name; }

		public override string ToString() => Name;
	}
}
