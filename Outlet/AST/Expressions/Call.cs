using System.Linq;

namespace Outlet.AST {
	public class Call : Expression {

		public Expression Caller { get; private set; }
		public readonly Expression[] Args;

		public Call(Expression caller, params Expression[] args) {
			Caller = caller;
			Args = args;
		}

        public void MakeConstructorCall()
        {
            Caller = new Deref(Caller, new Variable(""));
        }

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Caller.ToString() + "(" + Args.ToList().ToListString() + ")";

	}
}
