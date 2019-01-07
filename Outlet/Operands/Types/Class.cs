using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Decls = System.Collections.Generic.Dictionary<string, Outlet.Operands.Type>;

namespace Outlet.Operands {

	public class Class : Type {

		private readonly string Name;
		public readonly Decls Statics;
		public readonly Decls Instances;
		public StaticFunc SF;

		public Class(string name, Decls instances, Decls statics, StaticFunc sf) : base(Primitive.Object, null) {
			Name = name;
			Instances = instances;
			Statics = statics;
			SF = sf;
		}

		public override bool Is(Type t) => ReferenceEquals(this, t);

		public override bool Is(Type t, out int level) => throw new NotImplementedException();

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
