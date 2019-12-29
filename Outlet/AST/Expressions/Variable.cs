using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Variable : Expression {

		public readonly string Name;
        public int resolveLevel = -1;
        public int id = -1;
		public Variable(string name) {
			Name = name;
		}

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
		public override string ToString() => Name;
	}
}
