using System;
using Decls = System.Collections.Generic.Dictionary<string, Outlet.Operands.Type>;
using Outlet.Checking;

namespace Outlet.Operands {

	public abstract class Class : Type {

		public string Name;
		public Class Parent;

		public Class(string name, Class parent) {
			Name = name;
			Parent = parent;
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
        
		public override string ToString() => Name;
	}

	public class UserDefinedClass : Class, IRuntimeClass {

		private readonly Getter StaticGetter;
		private readonly Setter StaticSetter;
        private readonly Lister GetList;
		public readonly Action Init;

		public UserDefinedClass(string name, Class parent, Getter get, Setter set, Lister list, Action init) : base(name, parent) {
			Name = name;
			StaticGetter = get;
			StaticSetter = set;
            GetList = list;
            Init = init;
		}

		public Operand GetStatic(string s) => StaticGetter(s);
		public void SetStatic(string s, Operand val) => StaticSetter(s, val);

        public override string ToString()
        {
            string output = Name + "{\n";
            foreach(var (name, value) in GetList())
            {
                output += value.ToString() + " \n";
            }
            return output + "}";
        }
    }

	public class ProtoClass : Class, ICheckableClass {

		public readonly Decls Statics;
		public readonly Decls Instances;

		public ProtoClass(string name, Class parent, Decls instances, Decls statics) : base(name, parent) {
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
	
	public interface ICheckableClass {
		Type GetStaticType(string s);
		Type GetInstanceType(string s);
	}

	public interface IRuntimeClass {
		Operand GetStatic(string s);
		void SetStatic(string s, Operand val); 
	}
}
