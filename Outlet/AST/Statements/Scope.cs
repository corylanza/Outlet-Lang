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
            {"readline", new Native((Operand[] o) => new Literal(Console.ReadLine())) }
        };

		public Dictionary<string, Operand> Variables = new Dictionary<string, Operand>();
        public Dictionary<string, Function> Functions = new Dictionary<string, Function>();


        public List<Declaration> Lines = new List<Declaration>();
        private bool Repl = false;
		public Scope Parent;

		public Scope(Scope parent = null) {
			Parent = parent;
		}

        public Scope(bool replmode) {
            Repl = true;
        }

		public void AddVariable(Identifier id, Operand o) {
			if(Variables.ContainsKey(id.Name)) throw new OutletException("variable " + id.Name+" already exists in the current scope");
			Variables.Add(id.Name, o);
		}

        public void AddFunc(Identifier id, Function f) {
            if(Functions.ContainsKey(id.Name)) throw new OutletException("function " + id.Name + " already exists in the current scope");
            Functions.Add(id.Name, f);
        }

		public Operand Get(Identifier id) {
			if (Variables.ContainsKey(id.Name)) return Variables[id.Name];
			if (Parent != null) return Parent.Get(id);
			throw new OutletException("Cannot find variable " + id.Name); 
		}

        public Function GetFunc(Identifier id) {
            if(Functions.ContainsKey(id.Name)) return Functions[id.Name];
            if(Parent != null) return Parent.GetFunc(id);
            else if(NativeFunctions.ContainsKey(id.Name)) return NativeFunctions[id.Name];
            throw new OutletException("Cannot find function " + id.Name);
        }

		public void Execute() => Execute(this);


        public override void Execute(Scope block) {
			foreach (Declaration d in Lines) {
				if (d is Scope s) s.Execute();
				else d.Execute(block);
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
