using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decls = System.Collections.Generic.List<Outlet.AST.Declaration>;

namespace Outlet.AST {

	public class Class : Type, ICallable {
		
		private readonly List<Declaration> InstanceDecls;
		private Scope Scope;
		//private readonly List<Declaration> StaticDecls;
		
		public Class(string name, Scope closure, Decls instance, Decls statics) : base(name, Primitive.Object, null) {
			InstanceDecls = instance;
			Scope = new Scope(closure);
			foreach(Declaration d in statics) {
				d.Resolve(Scope);
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
			return new Instance(this, exec);//, ArgNames.TupleZip(args.ToList()));
		}

		public override Operand Dereference(Identifier field) {
			return Scope.Get(0, field.Name);
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override string ToString() {
			return Name;
		}
	}
}
