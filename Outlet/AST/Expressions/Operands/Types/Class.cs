using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decls = System.Collections.Generic.List<Outlet.AST.Declaration>;

namespace Outlet.AST {

	public class Class : Type, ICallable {

		private readonly string Name;
		private readonly List<Declaration> InstanceDecls;
		private Scope Scope;
		
		public Class(string name, Scope closure, Decls instance, Decls statics) : base(Primitive.Object, null) {
			Name = name;
			InstanceDecls = instance;
			Scope = new Scope(closure);
			foreach(Declaration d in statics) {
				d.Execute(Scope);
			}
		}

		// constructor
		public Operand Call(params Operand[] args) {
			Scope exec = new Scope(Scope);
			foreach(Declaration d in InstanceDecls) {
				d.Execute(exec);
			}
			(exec.Get(0, Name) as Function).Call(args);
			return new Instance(this, exec);
		}

		public override Operand Dereference(string field) {
			return Scope.Get(0, field);
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override string ToString() {
			return Name;
		}
	}
}
