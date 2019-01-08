using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decls = System.Collections.Generic.Dictionary<string, Outlet.Operands.Type>;
using Outlet.AST;

namespace Outlet.Operands {

	public class Class : Type {

		private readonly string Name;
		public Getter GetStatic;
		public Setter SetStatic;

		public Class(string name, Getter get, Setter set) : base(Primitive.Object, null) {
			Name = name;
			GetStatic = get;
			SetStatic = set;
		}

		public override bool Is(Type t) => t == Primitive.Object || ReferenceEquals(this, t);

		public override bool Is(Type t, out int level) => throw new NotImplementedException();

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override string ToString() => Name;

	}

	public class ProtoClass : Class {

		public readonly Decls Statics;
		public readonly Decls Instances;

		public ProtoClass(string name, Decls instances, Decls statics) : base(name, null, null) {
			Instances = instances;
			Statics = statics;
		}

	}

	public class NativeClass : Class {

		public readonly string Name;
		public readonly Dictionary<string, Operand> Methods = new Dictionary<string, Operand>();

		public NativeClass(string name, params (string, Operand)[] f) : base(name, null, null){
			Name = name;
			foreach(var v in f) Methods.Add(v.Item1, v.Item2);
		}

		public override string ToString() => Name;
	}
}
