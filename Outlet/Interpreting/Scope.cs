using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Type = Outlet.AST.Type;

namespace Outlet {
	public class Scope {

		private readonly Dictionary<string, (Type type, bool defined)> Defined = new Dictionary<string, (Type, bool)>();
		private readonly Dictionary<string, (Type Type, Operand Value)> Variables = new Dictionary<string, (Type, Operand)>();

		public readonly Scope Parent;

		public Scope(Scope parent = null) {
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
