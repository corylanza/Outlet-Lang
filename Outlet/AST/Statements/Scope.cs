using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Scope : Statement {

		public Dictionary<string, Operand> Variables = new Dictionary<string, Operand>();
		public List<Statement> Lines = new List<Statement>();
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
		public Operand Get(Identifier id) {
			if (Variables.ContainsKey(id.Name)) return Variables[id.Name];
			if (Parent != null) return Parent.Get(id);
			throw new OutletException("Cannot find variable " + id.Name); 
		}


		public override void Execute() {
			foreach (Statement e in Lines) e.Execute();
            if(!Repl) Variables.Clear();
		}

		public override string ToString() {
			string s = "{\n";
			foreach (Statement e in Lines) s += e.ToString()+'\n';
			return s+"}";
		}
	}
}
