using System.Linq;

namespace Outlet.AST {
	public class Call : Expression {

		public readonly Expression Caller;
		public readonly Expression[] Args;

		public Call(Expression caller, params Expression[] args) {
			Caller = caller;
			Args = args;
		}

		public override Operand Eval(Scope scope) {
			Operand Left = Caller.Eval(scope);
			if (Left is ICallable c) {
				return c.Call(Args.Select(arg => arg.Eval(scope)).ToArray());
			} else throw new OutletException(Left.Type.ToString() + " is not callable");
		}

        public override void Resolve(Scope scope) {
			Caller.Resolve(scope);
            foreach(Expression e in Args) e.Resolve(scope);
        }

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Caller.ToString() + "(" + Args.ToList().ToListString() + ")";

	}
}
