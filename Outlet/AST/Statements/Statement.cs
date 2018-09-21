using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public abstract class Statement {
		public abstract void Execute();

		public abstract override string ToString();
	}
}
