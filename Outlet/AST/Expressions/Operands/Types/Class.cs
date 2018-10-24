using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decls = System.Collections.Generic.List<Outlet.AST.Declaration>;

namespace Outlet.AST {

	public class Class : Type, ICallable {
		
		public readonly string Name;
		private readonly List<Identifier> ArgNames;
		private readonly List<Declaration> InstanceDecls;
		private Scope Scope;
		//private readonly Function Constructor;
		//private readonly List<Declaration> StaticDecls;
		//private readonly Dictionary<string, Operand> StaticFields;
		
		public Class(string name, Scope closure, List<Identifier> argnames, Decls instance, Decls statics) : base(name, Primitive.Object, null) {
			Name = name;
			ArgNames = argnames;
			InstanceDecls = instance;
			//StaticDecls = statics;
			Scope = new Scope(closure);
			foreach(Declaration d in statics) {
				d.Execute(Scope);
			}
			/*
			foreach(Declaration d in instance) {
				if(d is FunctionDeclaration f && f.ID == name) {
					
				} 
			}*/
		}

		// constructor
		public Operand Call(params Operand[] args) {
			Scope exec = new Scope(Scope);
			foreach(Declaration d in InstanceDecls) {
				if(d is FunctionDeclaration f && f.ID == Name) {
					var constructor = f.Construct(exec);
					constructor.Call();
				}
				d.Execute(exec);
			}
			return new Instance(this, exec, ArgNames.TupleZip(args.ToList()));
		}

		public Operand Get(Identifier field) {
			return null;
			//if (Methods.ContainsKey(field.Name)) return Methods[field.Name];
			//throw new OutletException("class does not have method, or it is inherited (not implemented)");
		}

		public override Operand Dereference(Identifier field) {
			return Scope.Get(0, field.Name);
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
