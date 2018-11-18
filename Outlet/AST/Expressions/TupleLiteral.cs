using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class TupleLiteral : Expression {

		public readonly Expression[] Args;

		public TupleLiteral(params Expression[] vals) {
			Args = vals;
		}
		/*
		public override Operand Eval(Scope scope) {
			if (Args.Length == 1) return Args[0].Eval(scope);
			IEnumerable<Operand> evaled = Args.Select(arg => arg.Eval(scope));
			if (evaled.All(t => t is Type)) return new TupleType(evaled.Select(t => t as Type).ToArray());
			else return new OTuple(evaled.ToArray());
		}

		public override void Resolve(Scope scope) {
			foreach (Expression e in Args) e.Resolve(scope);
		}*/

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => "("+Args.ToList().ToListString()+")";
	}
}
