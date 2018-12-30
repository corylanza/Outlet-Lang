using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Deref : Expression/*, IAssignable*/ {

		public readonly Expression Left;
		public readonly string Right;

		public Deref(Expression left, Expression right) {
			Left = left;
			if (right is Variable id) Right = id.Name;
			else throw new OutletException("expected identifier following dereferencing " + left.ToString());
		}
		/*
		public override Operand Eval(Scope scope) {
			Operand temp = Left.Eval(scope);
			if (temp is IDereferenceable derefed) return derefed.Dereference(Right);
			else throw new OutletException("cannot dereference variable of type " + temp.Type);
		}

		public override void Resolve(Scope scope) {
			Left.Resolve(scope);
		}
		
		public void Assign(Scope s, Operand value) {
			Operand i = Left.Eval(s);
			if(i is Instance instance) {
				instance.Assign(Right, value);
			} else throw new OutletException(Left.ToString()+ " is not an instance only instances have fields that can be assigned to");
		}*/

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Left.ToString() + "." + Right.ToString();
	}
}
