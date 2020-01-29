using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Variable : Expression, IVariable {

		public readonly string Name;
        public int resolveLevel = -1;
        public int LocalId { get; set; }
        public string Identifier => Name;


		public Variable(string name) {
			Name = name;
            LocalId = 1;
		}

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
		public override string ToString() => Name;
	}
}
