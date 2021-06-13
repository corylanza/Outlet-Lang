namespace Outlet.AST {
	public class VariableDeclaration : Declaration {

        public readonly Expression? Initializer;

		public VariableDeclaration(Declarator decl, Expression? initializer) : base(decl) {
			Initializer = initializer;
		}

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() {
			string s = Decl.ToString();
			if (Initializer is null) return s;
			else return s + " = " + Initializer.ToString();
		}
	}
}
