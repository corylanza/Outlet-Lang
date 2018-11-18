using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.Optimizing {
	public class ConstOptimizer : IVisitor<(bool, Const)> {

		public bool ConstCheck(Declaration d, out Const constant) {
			(bool b, Const c) = d.Accept(this);
			if(b) constant = c;
			else constant = null;
			return b;
		}



		public (bool, Const) Visit(ClassDeclaration c) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(FunctionDeclaration f) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(VariableDeclaration v) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(Const c) => (true, c);

		public (bool, Const) Visit(Assign a) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(Binary b) {
			b.Right.Accept(this);
			b.Left.Accept(this);
			return (false, null);
		}

		public (bool, Const) Visit(Call c) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(Declarator d) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(Deref d) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(Lambda l) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(ListLiteral l) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(ShortCircuit s) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(Ternary t) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(TupleLiteral t) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(Unary u) {
			if(ConstCheck(u.Expr, out Const c)) {
				u.Expr = c;
				return (true, (Const) u.Oper.Perform(c));
			}
			return (false, null);
		}

		public (bool, Const) Visit(Variable v) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(Block b) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(ForLoop f) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(IfStatement i) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(ReturnStatement r) {
			throw new NotImplementedException();
		}

		public (bool, Const) Visit(WhileLoop w) {
			throw new NotImplementedException();
		}
	}
}
