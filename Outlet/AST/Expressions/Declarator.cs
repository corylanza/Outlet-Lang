using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Declarator : Expression, IBindable
    {

		public readonly Expression Type;
		public string Identifier { get; private set; }
		public int ResolveLevel { get; private set; }
        public int LocalId { get; private set; }

		public Declarator(Expression type, string id) 
        {
			Type = type;
			Identifier = id;
            LocalId = -1;
            ResolveLevel = 0;
		}

        public void Bind(int id, int level)
        {
            LocalId = id;
            if (level != 0) throw new Exception("Cannot bind decl to resolve level other than 0");
        }

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => Type.ToString() + " " + Identifier;
	}
}
