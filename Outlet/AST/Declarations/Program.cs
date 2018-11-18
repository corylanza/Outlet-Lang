using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	class Program : Declaration {
		public override T Accept<T>(IVisitor<T> visitor) {
			throw new NotImplementedException();
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
