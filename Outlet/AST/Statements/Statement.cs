using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public abstract class Statement : IASTNode {

		public abstract T Accept<T>(IASTVisitor<T> visitor);
		public abstract override string ToString();
	}
}
