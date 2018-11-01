using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public abstract class Declaration {

		public abstract void Resolve(Scope scope);

		//public abstract Type TypeCheck();

		public abstract void Execute(Scope scope);

		public abstract override string ToString();
    }
}
