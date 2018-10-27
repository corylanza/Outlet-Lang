using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {
	public class Scope {

		private Dictionary<string, bool> Defined = new Dictionary<string, bool>();
		private Dictionary<string, (AST.Type Type, Operand Value)> Variables = new Dictionary<string, (AST.Type, Operand)>();

		public bool Repl = false;
		public Scope Parent;

		public Scope(Scope parent = null) {
			Parent = parent;
		}

		public Scope(bool replmode) {
			Repl = true;
		}

		public void Declare(string s) {
			if (Defined.ContainsKey(s)) throw new OutletException("variable " + s + " already declared in this scope");
			Defined.Add(s, false);
		}

		public void Define(string s) {
			Defined[s] = true;
		}

		public int Find(string s) {
			if (Defined.ContainsKey(s)) {
				if (Defined[s]) return 0;
				else {
					Defined.Remove(s);
					throw new OutletException("Cannot reference variable being initialized in its own initializer");
				}
			} else if (Parent != null) {
				int r = Parent.Find(s);
				if (r == -1) return -1;
				else return 1 + r;
			} else return -1;
		}

		public Operand Get(int level, string s) {
			if (level == 0) return Variables[s].Value;
			else return Parent.Get(level - 1, s);
		}

		public void Add(string id, AST.Type t, Operand v) {
			Variables[id] = (t, v);
		}

		public void Assign(int level, string id, Operand v) {
			if (level == 0) {
				AST.Type t = Variables[id].Type;
				if(v.Type.Is(t)) Variables[id] = (t, v);
				else throw new OutletException("cannot convert type " + v.Type.ToString() + " to type " + t.ToString());
			} else Parent.Assign(level - 1, id, v);
		}
	}
}
