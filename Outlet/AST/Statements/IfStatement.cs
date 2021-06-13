using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class IfStatement : Statement {

		public readonly Expression Condition;
        public readonly Statement Iftrue;
        public readonly Statement? Iffalse;

		public IfStatement(Expression condition, Statement iftrue, Statement? ifelse) =>
			(Condition, Iftrue, Iffalse) = (condition, iftrue, ifelse);
		
		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() => $"if({Condition}) {Iftrue}{(Iffalse is null ? "" : $" else {Iffalse}")}";
	}
}
