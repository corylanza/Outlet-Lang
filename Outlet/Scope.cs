using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {
	public class Scope {

		public Dictionary<string, bool> Defined = new Dictionary<string, bool>();
		public Dictionary<string, Operand> Variables = new Dictionary<string, Operand>();

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
			if (level == 0) return Variables[s];
			else return Parent.Get(level - 1, s);
		}

		public void Add(string id, Operand v) {
			Variables[id] = v;//.Add(id, v);
		}
	}
}
