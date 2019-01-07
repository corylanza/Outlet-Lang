using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public interface IASTNode {

		T Accept<T>(IVisitor<T> visitor);
	}
}
