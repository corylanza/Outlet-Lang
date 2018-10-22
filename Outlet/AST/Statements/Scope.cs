using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Scope : Statement {

		public Dictionary<string, Operand> Variables = new Dictionary<string, Operand>();
        public Dictionary<string, Function> Functions = new Dictionary<string, Function>();
        public Dictionary<string, bool> Defined = new Dictionary<string, bool>();

        public List<Declaration> Lines = new List<Declaration>();
        private bool Repl = false;
		public Scope Parent;

		public Scope(Scope parent = null) {
			Parent = parent;
		}

        public Scope(bool replmode) {
            Repl = true;
        }

        public void Declare(string s) {
            if(Defined.ContainsKey(s)) throw new OutletException("variable "+s+" already declared in this scope");
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
            if(level == 0) return Variables[s];
            else return Parent.Get(level - 1, s);
        }

		public void Add(string id, Operand v) {
			//if (Defined.ContainsKey(id) && Defined[id])
			Variables.Add(id, v);
			//else throw new OutletException("Variable "+id+" was never resolved");
		}

		public override void Resolve(Scope block) {
			foreach (Declaration s in Lines) s.Resolve(this);
		}

		public void Execute() => Execute(this);


        public override void Execute(Scope block) {
			foreach (Declaration d in Lines) {
				d.Execute(this);
			}
            if(!Repl) Variables.Clear();
		}

		public override string ToString() {
			string s = "{\n";
			foreach (Declaration d in Lines) s += d.ToString()+'\n';
			return s+"}";
		}
	}
}
