using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Type = Outlet.AST.Type;

namespace Outlet {
	public class Scope {

		public static Scope Global = new Scope();

		private readonly Dictionary<string, Type> DefinedTypes = new Dictionary<string, Type>();
		private readonly Dictionary<string, (Type type, bool defined)> Defined = new Dictionary<string, (Type, bool)>();
		private readonly Dictionary<string, (Type Type, Operand Value)> Variables = new Dictionary<string, (Type, Operand)>();

		public readonly Scope Parent;

		private Scope() {
			Parent = null;
			foreach(string s in ForeignFunctions.NativeFunctions.Keys) {
				Function f = ForeignFunctions.NativeFunctions[s];
				Define(f.Type, s);
				Add(s, f.Type, f);
			}
			foreach(string s in ForeignFunctions.NativeTypes.Keys) {
				Type t = ForeignFunctions.NativeTypes[s];
				DefineType(t, s);
				Add(s, Primitive.MetaType, t);
			}
		}

		public Scope(Scope parent) {
			Parent = parent;
		}

		public void Declare(Type t, string s) {
			if(Defined.ContainsKey(s)) throw new OutletException("variable " + s + " already declared in this scope");
			Defined.Add(s, (t, false));
		}

		public void Define(Type t, string s) {
			if(Defined.ContainsKey(s) && Defined[s].defined) throw new OutletException("variable " + s + " already defined in this scope");
			Defined[s] = (t, true);
		}

		public void DefineType(Type t, string name) {
			if(Defined.ContainsKey(name) && Defined[name].defined) throw new OutletException("type " + name + " already defined in this scope");
			DefinedTypes[name] = t;
			Defined[name] = (Primitive.MetaType, true);
		}

		public (Type, int) Find(string s) {
			if(Defined.ContainsKey(s)) {
				if(Defined[s].defined) return (Defined[s].type, 0);
				else {
					Defined.Remove(s);
					throw new OutletException("Cannot reference variable being initialized in its own initializer");
				}
			} else if(Parent != null) {
				(Type t, int r) = Parent.Find(s);
				if(r == -1) return (t, r);
				else return (t, 1 + r);
			} else return (null, -1);
		}

		public Type FindType(int level, string s) {
			if(level == 0) return DefinedTypes[s];
			else return Parent.FindType(level - 1, s);
		}

		public Operand Get(int level, string s) {
			if(level == 0) return Variables[s].Value;
			else return Parent.Get(level - 1, s);
		}

		public void Add(string id, Type t, Operand v) {
			Variables[id] = (t, v);
		}

		public void Assign(int level, string id, Operand v) {
			if(level == 0) {
				Type t = Variables[id].Type;
				if(v.Type.Is(t)) Variables[id] = (t, v);
				else throw new OutletException("cannot convert type " + v.Type.ToString() + " to type " + t.ToString());
			} else Parent.Assign(level - 1, id, v);
		}
	}
}
