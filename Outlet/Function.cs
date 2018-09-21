using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {
	public class Function {
		public Function() { }

		public virtual Operand Eval(params Operand[] args) {
			Console.WriteLine(args[0]);
			return null;
		}
	}
}
