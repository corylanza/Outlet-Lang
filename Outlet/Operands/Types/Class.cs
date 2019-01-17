using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decls = System.Collections.Generic.Dictionary<string, Outlet.Operands.Type>;
using Outlet.Checking;
using Outlet.AST;

namespace Outlet.Operands {

	public abstract class Class : Type {

		public string Name;
		public Class Parent;
		public object DefaultValue;

		public Class(string name, Class parent, object def) {
			Name = name;
			Parent = parent;
			DefaultValue = def;
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override bool Is(Type t) {
			if(Equals(t)) return true;
			if(!(Parent is null)) return Parent.Is(t);
			return false;
		}

		public override bool Is(Type t, out int level) {
			level = 0;
			if(Equals(t)) return true;
			if(!(Parent is null) && Parent.Is(t, out int l)) {
				level = l + 1;
				return true;
			}
			level = -1;
			return false;
		}

		public override dynamic Default() => DefaultValue;

		public override string ToString() => Name;
	}

	public class UserDefinedClass : Class, IRuntimeClass {

		private readonly Getter StaticGetter;
		private readonly Setter StaticSetter;
		public Action Init;

		public UserDefinedClass(string name, Class parent, Getter get, Setter set) : base(name, parent, null) {
			Name = name;
			StaticGetter = get;
			StaticSetter = set;
		}

		public Operand GetStatic(string s) => StaticGetter(s);
		public void SetStatic(string s, Operand val) => StaticSetter(s, val);

	}

	public class ProtoClass : Class, ICheckableClass {

		public readonly Decls Statics;
		public readonly Decls Instances;

		public ProtoClass(string name, Class parent, Decls instances, Decls statics) : base(name, parent, null) {
			Instances = instances;
			Statics = statics;
		}

		public Type GetStaticType(string s) {
			if(Statics.ContainsKey(s)) return Statics[s];
			if(s == "") return Checker.Error("type " + this + " is not instantiable");
			return Checker.Error(this + " does not contain static field: " + s);
		}
		public Type GetInstanceType(string s) {
			Class cur = this;
			while(cur != null) {
				if(cur is ProtoClass pc && pc.Instances.ContainsKey(s)) return pc.Instances[s];
				cur = cur.Parent;
			}
			return Checker.Error(this + " does not contain instance field: " + s);
		}
	}

	public class NativeClass : Class, IRuntimeClass, ICheckableClass {

		private readonly Dictionary<string, Operand> Methods = new Dictionary<string, Operand>();

		public NativeClass(string name, params (string, Operand)[] f) : base(name, null, null){
			Name = name;
			foreach(var v in f) Methods.Add(v.Item1, v.Item2);
		}

		public Operand GetStatic(string s) => Methods[s];
		public void SetStatic(string s, Operand val) => Methods[s] = val;
		public Type GetStaticType(string s) {
			if(Methods.ContainsKey(s)) return Methods[s].Type;
			if(s == "") return Checker.Error("type " + this + " is not instantiable");
			return Checker.Error(this + " does not contain static field: " + s);
		}
		public Type GetInstanceType(string s) => throw new NotImplementedException();
	}
	
	public interface ICheckableClass {
		Type GetStaticType(string s);
		Type GetInstanceType(string s);
	}

	public interface IRuntimeClass {
		Operand GetStatic(string s);
		void SetStatic(string s, Operand val); 
	}
}
