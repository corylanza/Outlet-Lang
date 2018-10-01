using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public abstract class Declaration {

		public abstract void Resolve();

		//public abstract void TypeCheck();

		public abstract void Execute(Scope block);

		public abstract override string ToString();
    }
}
