using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {

    public abstract class TokenLiteral : Token
    {
        public abstract override string ToString();
    }

    public class NullLiteral : TokenLiteral
    {
        public NullLiteral() { }

        public override string ToString() => "null";
    }

    public abstract class TokenLiteral<E> : TokenLiteral {

        protected TokenLiteral(E val)
        {
            Value = val;
        }

        [NotNull]
        public E Value;
        public override string ToString() => $"{Value}";
	}

	public class IntLiteral : TokenLiteral<int> {
		public IntLiteral(string value) : base(int.Parse(value)) { }
	}
	public class FloatLiteral : TokenLiteral<float> {
        public FloatLiteral(string value) : base(float.Parse(value)) { }
	}

	public class BoolLiteral : TokenLiteral<bool> {
		public BoolLiteral(string value) : base(bool.Parse(value)) { }
	}

	public class StringLiteral : TokenLiteral<string> {
		public StringLiteral(string value) : base(value) {}
	}
}
