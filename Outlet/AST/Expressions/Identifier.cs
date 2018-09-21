using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Identifier : Expression, IToken {
		public string Name;
		private Scope Scope;
		public Identifier(string name) { Name = name; }

		public void SetScope(Scope s) => Scope = s;

		public override Operand Eval() => Scope.Get(this);

		public override string ToString() => Name;
	}
}
