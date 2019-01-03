using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Decls = System.Collections.Generic.List<Outlet.AST.Declaration>;

namespace Outlet.Operands {

	public class Class : Type {

		private readonly string Name;
		private readonly List<Declaration> InstanceDecls;
		private Scope Scope;
		//public Dictionary<string, Type> Statics = new Dictionary<string, Type>();
		//public Dictionary<string, Type> Instances = new Dictionary<string, Type>();

		public Class(string name, Scope closure, Decls instance, Decls statics, StaticFunc sf) : base(Primitive.Object, null) {
			Name = name;
			InstanceDecls = instance;
			Scope = closure;
			//Scope = new Scope(closure);
		}

		/* constructor
		public Operand Call(params Operand[] args) {
			Scope exec = new Scope(Scope);
			foreach(Declaration d in InstanceDecls) {
				//d.Execute(exec);
			}
			//(exec.Get(0, Name) as Function).Call(args);
			return new Instance(this, exec);
		}*/

		public override bool Is(Type t) => ReferenceEquals(this, t);

		public override bool Is(Type t, out int level) => throw new NotImplementedException();

		//public Operand Dereference(string field) => Scope.Get(0, field);

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override string ToString() => Name;

	}

	public class NativeClass : Type {

		public readonly string Name;
		public readonly Dictionary<string, Operand> Methods = new Dictionary<string, Operand>();

		public NativeClass(string name, params (string, Operand)[] f) : base(Primitive.Object, null){
			Name = name;
			foreach(var v in f) Methods.Add(v.Item1, v.Item2);
		}

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
		}

		public override bool Is(Type t) => ReferenceEquals(this, t);

		public override bool Is(Type t, out int level) {
			throw new NotImplementedException();
		}

		public override string ToString() => Name;
	}
}
