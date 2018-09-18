using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Expressions {
	public class Identifier : Expression, IToken {
		public string Name;
		public Identifier(string name) { Name = name; }

		public override Operand Eval() => throw new NotImplementedException();

		public override string ToString() => Name;
	}
}
