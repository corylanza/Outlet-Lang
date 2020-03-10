using System;
using Outlet.Types;

namespace Outlet.AST {

    public abstract class Literal : Expression
    {

    }

    public class Literal<E> : Literal {

		public Primitive Type;
		public E Value;

		public Literal() {
			Value = default;
            Type = Primitive.Object;
        }

        public Literal(E value) {
            Type = value switch
            {
                int _ => Primitive.Int,
                bool _ => Primitive.Bool,
                float _ => Primitive.Float,
                string _ => Primitive.String,
                _ => throw new UnexpectedException("not a primitive")
            };
			Value = value;
		}


		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => Value?.ToString() ?? "null";
	}
}
