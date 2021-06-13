using Outlet.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Declarator : Expression, IBindable
    {

		public readonly Expression? Type;
        public bool IsVar => Type is null;
        public bool IsOperatorOverload => Identifier == "operator";
		public string Identifier { get; private set; }
		public uint? ResolveLevel { get; private set; }
        public uint? LocalId { get; private set; }

		public Declarator(Expression type, string id) 
        {
			Type = type;
			Identifier = id;
            LocalId = null;
            ResolveLevel = null;
		}

        public Declarator(string id)
        {
            Type = null;
            Identifier = id;
            LocalId = null;
            ResolveLevel = null;
        }

        public void Bind(uint id, uint level)
        {
            LocalId = id;
            if (level != 0) throw new UnexpectedException("Cannot bind decl to resolve level other than 0");
            ResolveLevel = 0;
        }

        public Declarator CreateOperatorBinding(Operator op) => new Declarator(Type, op.ToString());

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => (Type?.ToString() ?? "var") + " " + Identifier;
	}
}
