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
            Caller = new MemberAccess(Caller, new Variable(""));
        }

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"{Caller}({string.Join(",", Args.Select(arg => arg.ToString()))})";

	}
}
