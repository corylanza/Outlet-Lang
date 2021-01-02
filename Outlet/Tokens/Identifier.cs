using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {
	public class Identifier : Token {

		public readonly string Name;

		public Identifier(string name) {
			Name = name;
		}

		public override bool Equals(object? obj) => obj is Identifier id && id.Name == Name;
		public override int GetHashCode() => base.GetHashCode();
		public override string ToString() => Name;
	}
}
