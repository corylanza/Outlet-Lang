using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Expressions {
	public class Scope : Expression {

		public List<Expression> Lines = new List<Expression>();

		public Scope() { }


		public override Operand Eval() {
			foreach (Expression e in Lines) e.Eval();
			return null;
		}

		public override string ToString() {
			string s = "{\n";
			foreach (Expression e in Lines) s += e.ToString()+'\n';
			return s+"}";
		}
	}
}
