using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Variable : Expression, IBindable {

		public string Identifier { get; private set; }
        public uint? ResolveLevel { get; private set; }
        public uint? LocalId { get; private set; }


		public Variable(string name) {
			Identifier = name;
            ResolveLevel = null;
            LocalId = null;
		}

        public void Bind(uint id, uint level) => (LocalId, ResolveLevel) = (id, level);

        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);
		public override string ToString() => Identifier;
	}
}
