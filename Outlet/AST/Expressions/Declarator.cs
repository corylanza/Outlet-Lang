using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Declarator : Expression, IDeclarable
    {

		public readonly Expression Type;
		public string Identifier { get; private set; }
        public int LocalId { get; set; }

		public Declarator(Expression type, string id) 
        {
			Type = type;
			Identifier = id;
            LocalId = -1;
		}

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => Type.ToString() + " " + Identifier;
	}
}
