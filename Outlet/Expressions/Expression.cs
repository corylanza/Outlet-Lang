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

    public abstract class Operand : Expression {
        public object Value;

        public override Operand Eval() => this;
    }

    public class Literal : Operand {
        public Literal(object value) {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
