using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Scope : Statement {

		public static Dictionary<string, Function> NativeFunctions = new Dictionary<string, Function>() {
			{"print", new Native((Operand[] o) => {
									foreach(Operand op in o){
										Console.WriteLine(op.ToString());
									} return null; }) },
			{"readline", new Native((Operand[] o) => new Literal(Console.ReadLine())) },
			{"max", new Native((Operand[] o) => new Literal(o.Max(x => x.Value))) },
			{"type", new Native((Operand[] o) => new Literal(o[0].Type.Name)) }
		};

		public static Dictionary<string, Type> NativeTypes = new Dictionary<string, Type>() {
			{"list", Type.List }
		};

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
            if(Defined.ContainsKey(s)) {
                if(Defined[s]) return 0;
                else {
                    Defined.Remove(s);
                    throw new OutletException("Cannot reference variable being initialized in its own initializer");
                }
            } else if(Parent != null) return 1 + Parent.Find(s);
            else return -1;
        }

        public Operand Get(int level, string s) {
            if(level == 0) return Variables[s];
            else return Parent.Get(level - 1, s);
        }

		public void AddVariable(string id, Operand o) {
			if(Variables.ContainsKey(id)) throw new OutletException("variable " + id+" already exists in the current scope");
			Variables.Add(id, o);
		}

        public void AddFunc(string id, Function f) {
            if(Functions.ContainsKey(id)) throw new OutletException("function " + id + " already exists in the current scope");
            Functions.Add(id, f);
        }
        /*
		public Operand Get(string id) {
			if (Variables.ContainsKey(id)) return Variables[id];
			if (Parent != null) return Parent.Get(id);
			else if (NativeTypes.ContainsKey(id)) return NativeTypes[id];
			throw new OutletException("Cannot find variable " + id); 
		}*/

        public Function GetFunc(string id) {
            if(Functions.ContainsKey(id)) return Functions[id];
            if(Parent != null) return Parent.GetFunc(id);
            else if(NativeFunctions.ContainsKey(id)) return NativeFunctions[id];
            throw new OutletException("Cannot find function " + id);
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
