using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {

	public class Class : Type, ICallable {
		
		private List<Identifier> ArgNames;

		public Class(string name, List<Identifier> argnames) : base(name, Primitive.Object, null) {
			ArgNames = argnames;
		}

		// constructor
		public Operand Call(params Operand[] args) {
			return new Instance(this, ArgNames.TupleZip(args.ToList()));
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override Operand Eval(Scope scope) => this;

		public override string ToString() {
			if (ArgNames.Count == 0) return Name + "()";
			string s = Name + "(";
			foreach (Identifier id in ArgNames) {
				s += id.ToString() + ", ";
			}
			return s.Substring(0, s.Length - 2) + ")";
		}
	}
}
