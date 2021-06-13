using System;
using Outlet.Types;

namespace Outlet.AST {

    public abstract class Literal : Expression
    {

    }

    public class Literal<E> : Literal where E : struct {

		public Primitive Type;
		public E Value;

        public Literal(E value) {
            Type = value switch
            {
                int _ => Primitive.Int,
                bool _ => Primitive.Bool,
                float _ => Primitive.Float,
                _ => throw new UnexpectedException("not a primitive")
            };
			Value = value;
		}


		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => Value.ToString() ?? throw new Exception("Invalid value");
	}

    public class StringLiteral : Literal
    {
        public string Value;

        public StringLiteral(string s) => Value = s;

        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => Value;
    }

    public class NullExpr : Literal
    {
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => "null";
    }
}
