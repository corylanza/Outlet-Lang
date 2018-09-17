using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Expressions {
    public abstract class Expression {

        public abstract Operand Eval();

        public abstract override string ToString();
    }

	public class Type {
		public Type(string name) {

		}
	}
	/*
	public class Primitive : Type {
		private Primitive(string name)
	}*/

    public abstract class Operand : Expression {
        public object Value;

        public override Operand Eval() => this;
    }

    public class Literal : Operand {
        public Literal(int value) {
            Value = value;
			//Type = Primitive.Int;
        }

        public override string ToString() => Value.ToString();
    }
}
