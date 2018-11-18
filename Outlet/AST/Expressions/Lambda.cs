using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Lambda : Expression {

		private readonly Expression Left, Right;

		public Lambda(Expression l, Expression r) {
			Left = l;
			Right = r;
		}
		/*
		public override Operand Eval(Scope scope) {
			Operand l = Left.Eval(scope);
			Operand r = Right.Eval(scope);
			if(l is Type lt && r is Type rt) {
				//return new FunctionType(lt, rt);
			}
			//Function f = new Function(scope, "", )
			throw new NotImplementedException();
		}

		public override void Resolve(Scope scope) {
			Left.Resolve(scope);
			Right.Resolve(scope);
			//Scope exec = new Scope(scope);
		}*/

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() {
			throw new NotImplementedException();
		}
	}
}
