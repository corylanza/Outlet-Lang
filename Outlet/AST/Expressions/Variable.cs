using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Variable : Expression, IBindable {

		public string Identifier { get; private set; }
        public int ResolveLevel { get; private set; }
        public int LocalId { get; private set; }


		public Variable(string name) {
			Identifier = name;
            ResolveLevel = -1;
            LocalId = -1;
		}

        public void Bind(int id, int level) => (LocalId, ResolveLevel) = (id, level);

        public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
		public override string ToString() => Identifier;
	}
}
