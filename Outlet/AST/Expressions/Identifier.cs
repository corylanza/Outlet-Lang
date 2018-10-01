using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Identifier : Expression, IToken {
		public string Name;
		public Identifier(string name) { Name = name; }

		public override Operand Eval(Scope block) => block.Get(this);

		public override string ToString() => Name;
	}
}
